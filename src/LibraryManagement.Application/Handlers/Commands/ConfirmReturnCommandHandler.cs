using LibraryManagement.Application.Commands.Loans;
using LibraryManagement.Domain.Enums;
using LibraryManagement.Domain.Interfaces;
using MediatR;

namespace LibraryManagement.Application.Handlers.Commands;

public class ConfirmReturnCommandHandler : IRequestHandler<ConfirmReturnCommand, ConfirmReturnResult>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IEmailService _emailService;

    public ConfirmReturnCommandHandler(IUnitOfWork unitOfWork, IEmailService emailService)
    {
        _unitOfWork = unitOfWork;
        _emailService = emailService;
    }

    public async Task<ConfirmReturnResult> Handle(ConfirmReturnCommand request, CancellationToken cancellationToken)
    {
        var loan = await _unitOfWork.Loans.GetByIdAsync(request.LoanId);
        if (loan == null)
            return new ConfirmReturnResult { Success = false, Message = "Pozajmica ne postoji." };

        if (loan.Status != LoanStatus.PendingReturn)
            return new ConfirmReturnResult { Success = false, Message = "Vraćanje nije na čekanju." };

        var copy = await _unitOfWork.BookCopies.GetByIdAsync(loan.BookCopyId);
        if (copy == null)
            return new ConfirmReturnResult { Success = false, Message = "Primerak ne postoji." };

        var book = await _unitOfWork.Books.GetByIdAsync(copy.BookId);
        if (book == null)
            return new ConfirmReturnResult { Success = false, Message = "Knjiga ne postoji." };

        var user = await _unitOfWork.Users.FirstOrDefaultAsync(u => u.Id == loan.UserId);
        if (user == null)
            return new ConfirmReturnResult { Success = false, Message = "Korisnik ne postoji." };


        try
        {
            await _unitOfWork.BeginTransactionAsync();

            // Potvrdi vraćanje
            loan.Status = LoanStatus.Returned;
            loan.ReturnConfirmedByUserId = request.LibrarianUserId;
            loan.ReturnConfirmedAt = DateTime.Now;

            // Obračunaj kaznu (ako kasni)
            if (loan.ReturnDate > loan.DueDate)
            {
                var daysLate = (loan.ReturnDate.Value - loan.DueDate.Value).Days;
                loan.FineAmount = daysLate * 50m; // 50 dinara po danu

                user.TotalFines += loan.FineAmount ?? 0m;
                _unitOfWork.Users.Update(user);
            }

            // Vrati primerak u Available
            copy.Status = BookCopyStatus.Available;
            _unitOfWork.BookCopies.Update(copy);

            // Povećaj AvailableCopies
            book.AvailableCopies++;
            _unitOfWork.Books.Update(book);

            _unitOfWork.Loans.Update(loan);

            // PROVERI RED ČEKANJA (kao u ReturnBookCommandHandler)
            var pendingReservations = await _unitOfWork.Reservations.FindAsync(
                r => r.BookId == book.Id && r.Status == ReservationStatus.Pending
            );

            var firstInQueue = pendingReservations
                .OrderBy(r => r.QueuePosition)
                .FirstOrDefault();

            if (firstInQueue != null)
            {
                firstInQueue.Status = ReservationStatus.Ready;
                firstInQueue.ExpirationDate = DateTime.Now.AddDays(3);  // ← OVDE POČINJE 3 dana!
                _unitOfWork.Reservations.Update(firstInQueue);

                // Pošalji email korisniku koji je sledeći u redu za rezervaciju
                var reservationUser = await _unitOfWork.Users.FirstOrDefaultAsync(u => u.Id == firstInQueue.UserId);
                if (reservationUser?.Email != null)
                {
                    try
                    {
                        await _emailService.SendEmailAsync(
                            reservationUser.Email,
                            "Rezervacija spremna za preuzimanje - Biblioteka",
                            $@"<h2>Poštovani {reservationUser.FirstName} {reservationUser.LastName},</h2>
                            <p>Vaša rezervacija je <strong>spremna za preuzimanje</strong>.</p>
                            <p><strong>Knjiga:</strong> {book.Title} - {book.Author}</p>
                            <p><strong>Rok za preuzimanje:</strong> {firstInQueue.ExpirationDate:dd.MM.yyyy}</p>
                            <p>Molimo preuzmite knjigu u roku od 3 dana, nakon čega rezervacija ističe.</p>
                            <br/>
                            <p>Srdačan pozdrav,<br/>Vaša biblioteka</p>");
                    }
                    catch
                    {
                        // Email greška ne treba da spreči ažuriranje rezervacije
                    }
                }

                // Pomeri ostale u redu
                var otherReservations = pendingReservations
                    .Where(r => r.QueuePosition > firstInQueue.QueuePosition)
                    .ToList();

                foreach (var reservation in otherReservations)
                {
                    reservation.QueuePosition--;
                    _unitOfWork.Reservations.Update(reservation);
                }
            }

            await _unitOfWork.SaveChangesAsync();
            await _unitOfWork.CommitTransactionAsync();

            // Pošalji email obaveštenje o potvrđenom vraćanju
            if (user.Email != null)
            {
                try
                {
                    var fineInfo = loan.FineAmount > 0
                        ? $"<p><strong>Kazna za kašnjenje:</strong> {loan.FineAmount} dinara</p>"
                        : "";

                    await _emailService.SendEmailAsync(
                        user.Email,
                        "Vraćanje knjige potvrđeno - Biblioteka",
                        $@"<h2>Poštovani {user.FirstName} {user.LastName},</h2>
                        <p>Vraćanje knjige je <strong>potvrđeno</strong>.</p>
                        <p><strong>Knjiga:</strong> {book.Title} - {book.Author}</p>
                        {fineInfo}
                        <p>Hvala Vam na korišćenju naše biblioteke.</p>
                        <br/>
                        <p>Srdačan pozdrav,<br/>Vaša biblioteka</p>");
                }
                catch
                {
                    // Email greška ne treba da spreči potvrdu vraćanja
                }
            }

            return new ConfirmReturnResult
            {
                Success = true,
                Message = "Vraćanje potvrđeno.",
                FineAmount = loan.FineAmount ?? 0m
            };
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackTransactionAsync();
            return new ConfirmReturnResult { Success = false, Message = ex.Message };
        }
    }
}