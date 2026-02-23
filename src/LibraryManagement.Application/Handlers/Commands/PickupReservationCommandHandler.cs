using LibraryManagement.Application.Commands.Reservations;
using LibraryManagement.Domain.Entities;
using LibraryManagement.Domain.Enums;
using LibraryManagement.Domain.Interfaces;
using MediatR;

namespace LibraryManagement.Application.Handlers.Commands;

public class PickupReservationCommandHandler : IRequestHandler<PickupReservationCommand, PickupReservationResult>
{
    private readonly IUnitOfWork _unitOfWork;

    public PickupReservationCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<PickupReservationResult> Handle(PickupReservationCommand request, CancellationToken cancellationToken)
    {
        // 1. Pronađi rezervaciju
        var reservation = await _unitOfWork.Reservations.GetByIdAsync(request.ReservationId);
        if (reservation == null)
            return new PickupReservationResult { Success = false, Message = "Rezervacija ne postoji." };

        // 2. Proveri da li je korisnik vlasnik rezervacije
        if (reservation.UserId != request.UserId)
            return new PickupReservationResult { Success = false, Message = "Ovo nije vaša rezervacija." };

        // 3. Proveri da li je rezervacija spremna
        if (reservation.Status != ReservationStatus.Ready)
            return new PickupReservationResult { Success = false, Message = "Rezervacija nije spremna za preuzimanje." };

        // 4. Proveri da li je istekla (prošlo više od 3 dana)
        if (reservation.ExpirationDate.HasValue && DateTime.Now > reservation.ExpirationDate.Value)
        {
            // Automatski postavi status na Expired
            reservation.Status = ReservationStatus.Expired;
            _unitOfWork.Reservations.Update(reservation);
            await _unitOfWork.SaveChangesAsync();

            return new PickupReservationResult
            {
                Success = false,
                Message = "Rezervacija je istekla. Rok za preuzimanje je prošao."
            };
        }

        // 5. Pronađi knjigu
        var book = await _unitOfWork.Books.GetByIdAsync(reservation.BookId);
        if (book == null)
            return new PickupReservationResult { Success = false, Message = "Knjiga ne postoji." };

        // 6. Pronađi dostupan primerak
        var availableCopy = await _unitOfWork.BookCopies.FirstOrDefaultAsync(
            c => c.BookId == book.Id && c.Status == BookCopyStatus.Available
        );

        if (availableCopy == null)
            return new PickupReservationResult
            {
                Success = false,
                Message = "Nema dostupnih primeraka. Molimo kontaktirajte bibliotekara."
            };

        try
        {
            await _unitOfWork.BeginTransactionAsync();

            // 7. Kreiraj pozajmicu sa statusom PENDING (čeka odobrenje!)
            var loan = new Loan
            {
                BookCopyId = availableCopy.Id,
                UserId = request.UserId,
                LoanDate = DateTime.Now,
                DueDate = null,  // Postavlja se tek kada bibliotekar odobri
                Status = LoanStatus.Pending,  // Čeka odobrenje!
                FineAmount = 0,
                FinePaid = false,
                CreatedAt = DateTime.Now
            };

            await _unitOfWork.Loans.AddAsync(loan);

            // 8. Označi rezervaciju kao "Picked Up" (preuzetu)
            reservation.Status = ReservationStatus.PickedUp;  // NOVI STATUS!
            _unitOfWork.Reservations.Update(reservation);

            await _unitOfWork.SaveChangesAsync();
            await _unitOfWork.CommitTransactionAsync();

            return new PickupReservationResult
            {
                Success = true,
                Message = "Zahtev za pozajmicu je poslat. Čeka se odobrenje bibliotekara.",
                LoanId = loan.Id
            };
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackTransactionAsync();
            return new PickupReservationResult { Success = false, Message = ex.Message };
        }
    }
}