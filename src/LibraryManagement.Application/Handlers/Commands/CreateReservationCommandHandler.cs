using LibraryManagement.Application.Commands.Reservations;
using LibraryManagement.Domain.Entities;
using LibraryManagement.Domain.Enums;
using LibraryManagement.Domain.Interfaces;
using MediatR;

namespace LibraryManagement.Application.Handlers.Commands;

public class CreateReservationCommandHandler : IRequestHandler<CreateReservationCommand, CreateReservationResult>
{
    private readonly IUnitOfWork _unitOfWork;
    private const int AverageLoanDays = 14;

    public CreateReservationCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<CreateReservationResult> Handle(CreateReservationCommand request, CancellationToken cancellationToken)
    {
        // Provera da li korisnik postoji
        var user = await _unitOfWork.Users.FirstOrDefaultAsync(u => u.Id == request.UserId);
        if (user == null)
        {
            return new CreateReservationResult
            {
                Success = false,
                Message = "Korisnik ne postoji."
            };
        }

        // Provera da li je korisnik blokiran
        if (user.IsBlocked)
        {
            return new CreateReservationResult
            {
                Success = false,
                Message = "Vaš nalog je blokiran."
            };
        }

        // Provera da li knjiga postoji
        var book = await _unitOfWork.Books.GetByIdAsync(request.BookId);
        if (book == null)
        {
            return new CreateReservationResult
            {
                Success = false,
                Message = "Knjiga ne postoji."
            };
        }

        // ✅ PROVERA DOSTUPNOSTI - ODMAH NA POČETKU!
        if (book.AvailableCopies > 0)
        {
            return new CreateReservationResult
            {
                Success = false,
                Message = "Knjiga je trenutno dostupna! Možete je odmah pozajmiti."
            };
        }

        // Provera da li korisnik već ima aktivnu rezervaciju za ovu knjigu
        var existingReservation = await _unitOfWork.Reservations.FirstOrDefaultAsync(
            r => r.BookId == request.BookId &&
                 r.UserId == request.UserId &&
                 (r.Status == ReservationStatus.Pending || r.Status == ReservationStatus.Ready));

        if (existingReservation != null)
        {
            return new CreateReservationResult
            {
                Success = false,
                Message = "Već imate aktivnu rezervaciju za ovu knjigu."
            };
        }

        // Provera da li korisnik trenutno pozajmljuje ovu knjigu
        var activeLoans = await _unitOfWork.Loans.FindAsync(
            l => l.UserId == request.UserId &&
                 (l.Status == LoanStatus.Active || l.Status == LoanStatus.Approved));

        foreach (var loan in activeLoans)
        {
            var bookCopy = await _unitOfWork.BookCopies.GetByIdAsync(loan.BookCopyId);
            if (bookCopy?.BookId == request.BookId)
            {
                return new CreateReservationResult
                {
                    Success = false,
                    Message = "Trenutno pozajmljujete ovu knjigu."
                };
            }
        }

        try
        {
            await _unitOfWork.BeginTransactionAsync();

            // Određivanje pozicije u redu čekanja
            var existingReservations = await _unitOfWork.Reservations
                .FindAsync(r => r.BookId == request.BookId && r.Status == ReservationStatus.Pending);

            var reservationsList = existingReservations.ToList();

            int queuePosition = reservationsList.Count > 0
                ? reservationsList.Max(r => r.QueuePosition) + 1
                : 1;

            // ✅ Procena vremena dostupnosti
            int estimatedDays = queuePosition * AverageLoanDays;

            // Kreiranje rezervacije
            var reservation = new Reservation
            {
                BookId = request.BookId,
                UserId = request.UserId,
                ReservationDate = DateTime.Now,
                Status = ReservationStatus.Pending,
                QueuePosition = queuePosition,
                CreatedAt = DateTime.Now
            };

            await _unitOfWork.Reservations.AddAsync(reservation);
            await _unitOfWork.SaveChangesAsync();
            await _unitOfWork.CommitTransactionAsync();

            return new CreateReservationResult
            {
                Success = true,
                Message = $"Rezervacija je uspešno kreirana! Vaša pozicija u redu: {queuePosition}. Procenjeno vreme čekanja: {estimatedDays} dana.",
                ReservationId = reservation.Id,
                QueuePosition = queuePosition
            };
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackTransactionAsync();
            return new CreateReservationResult
            {
                Success = false,
                Message = $"Greška prilikom kreiranja rezervacije: {ex.Message}"
            };
        }
    }
}