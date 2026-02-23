using LibraryManagement.Application.DTOs;
using LibraryManagement.Application.Queries.Reservations;
using LibraryManagement.Domain.Enums;
using LibraryManagement.Domain.Interfaces;
using MediatR;

namespace LibraryManagement.Application.Handlers.Queries;

public class GetReservationQueueQueryHandler : IRequestHandler<GetReservationQueueQuery, IEnumerable<ReservationDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetReservationQueueQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<ReservationDto>> Handle(GetReservationQueueQuery request, CancellationToken cancellationToken)
    {
        var reservations = (await _unitOfWork.Reservations
            .FindAsync(r => r.BookId == request.BookId && 
                           (r.Status == ReservationStatus.Pending || r.Status == ReservationStatus.Ready)))
            .OrderBy(r => r.QueuePosition);

        var reservationDtos = new List<ReservationDto>();
        foreach (var reservation in reservations)
        {
            var book = await _unitOfWork.Books.GetByIdAsync(reservation.BookId);
            var user = await _unitOfWork.Users.FirstOrDefaultAsync(u => u.Id == reservation.UserId);

            reservationDtos.Add(new ReservationDto
            {
                Id = reservation.Id,
                BookId = reservation.BookId,
                BookTitle = book?.Title ?? "N/A",
                BookAuthor = book?.Author ?? "N/A",
                UserName = user != null ? $"{user.FirstName} {user.LastName}" : "N/A",
                ReservationDate = reservation.ReservationDate,
                ExpirationDate = reservation.ExpirationDate,
                Status = reservation.Status,
                QueuePosition = reservation.QueuePosition,
                NotifiedDate = reservation.NotifiedDate
            });
        }

        return reservationDtos;
    }
}
