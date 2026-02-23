using LibraryManagement.Application.DTOs;
using MediatR;

namespace LibraryManagement.Application.Queries.Reservations;

public class GetUserReservationsQuery : IRequest<IEnumerable<ReservationDto>>
{
    public string UserId { get; set; } = string.Empty;
}
