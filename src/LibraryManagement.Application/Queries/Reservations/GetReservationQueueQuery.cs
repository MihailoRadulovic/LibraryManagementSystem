using LibraryManagement.Application.DTOs;
using MediatR;

namespace LibraryManagement.Application.Queries.Reservations;

public class GetReservationQueueQuery : IRequest<IEnumerable<ReservationDto>>
{
    public int BookId { get; set; }
}
