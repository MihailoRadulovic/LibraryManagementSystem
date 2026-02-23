using MediatR;

namespace LibraryManagement.Application.Commands.Reservations;

public class CreateReservationCommand : IRequest<CreateReservationResult>
{
    public int BookId { get; set; }
    public string UserId { get; set; } = string.Empty;
}

public class CreateReservationResult
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public int? ReservationId { get; set; }
    public int QueuePosition { get; set; }
    public DateTime? EstimatedAvailability { get; set; }
}
