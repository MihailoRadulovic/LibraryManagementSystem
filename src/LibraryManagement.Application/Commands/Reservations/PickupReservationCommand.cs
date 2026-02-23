using MediatR;

namespace LibraryManagement.Application.Commands.Reservations;

public class PickupReservationCommand : IRequest<PickupReservationResult>
{
    public int ReservationId { get; set; }
    public string UserId { get; set; } = string.Empty;
}

public class PickupReservationResult
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public int? LoanId { get; set; }
}