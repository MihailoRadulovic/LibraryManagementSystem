using LibraryManagement.Application.Commands.Reservations;
using LibraryManagement.Domain.Enums;
using LibraryManagement.Domain.Interfaces;
using MediatR;

namespace LibraryManagement.Application.Handlers.Commands;

public class CancelReservationCommandHandler : IRequestHandler<CancelReservationCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public CancelReservationCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(CancelReservationCommand request, CancellationToken cancellationToken)
    {
        var reservation = await _unitOfWork.Reservations.GetByIdAsync(request.ReservationId);
        
        if (reservation == null)
        {
            throw new InvalidOperationException("Rezervacija ne postoji.");
        }

        if (reservation.UserId != request.UserId)
        {
            throw new UnauthorizedAccessException("Nemate dozvolu da otkažete ovu rezervaciju.");
        }

        if (reservation.Status == ReservationStatus.PickedUp || 
            reservation.Status == ReservationStatus.Cancelled)
        {
            throw new InvalidOperationException("Rezervacija ne može biti otkazana.");
        }

        try
        {
            await _unitOfWork.BeginTransactionAsync();

            reservation.Status = ReservationStatus.Cancelled;
            reservation.UpdatedAt = DateTime.Now;
            _unitOfWork.Reservations.Update(reservation);

            // Ažuriranje pozicija u redu za ostale rezervacije
            var otherReservations = (await _unitOfWork.Reservations
                .FindAsync(r => r.BookId == reservation.BookId && 
                               r.Status == ReservationStatus.Pending &&
                               r.QueuePosition > reservation.QueuePosition))
                .OrderBy(r => r.QueuePosition)
                .ToList();

            foreach (var other in otherReservations)
            {
                other.QueuePosition--;
                other.UpdatedAt = DateTime.Now;
                _unitOfWork.Reservations.Update(other);
            }

            await _unitOfWork.SaveChangesAsync();
            await _unitOfWork.CommitTransactionAsync();

            return true;
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync();
            throw;
        }
    }
}
