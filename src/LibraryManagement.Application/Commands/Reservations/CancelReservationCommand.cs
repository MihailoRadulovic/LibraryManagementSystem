using MediatR;

namespace LibraryManagement.Application.Commands.Reservations;

public class CancelReservationCommand : IRequest<bool>
{
    public int ReservationId { get; set; }
    public string UserId { get; set; } = string.Empty;
}
