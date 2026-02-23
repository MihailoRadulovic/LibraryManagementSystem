using LibraryManagement.Application.DTOs;
using LibraryManagement.Application.Queries.Books;
using LibraryManagement.Domain.Interfaces;
using MediatR;
using AutoMapper;

namespace LibraryManagement.Application.Handlers.Queries;

public class GetBookByIdQueryHandler : IRequestHandler<GetBookByIdQuery, BookDto?>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetBookByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<BookDto?> Handle(GetBookByIdQuery request, CancellationToken cancellationToken)
    {
        var book = await _unitOfWork.Books.GetByIdAsync(request.Id);
        
        if (book == null)
        {
            return null;
        }
        return _mapper.Map<BookDto>(book);

        //var reservationCount = await _unitOfWork.Reservations.CountAsync(
        //    r => r.BookId == book.Id && 
        //         (r.Status == Domain.Enums.ReservationStatus.Pending || 
        //          r.Status == Domain.Enums.ReservationStatus.Ready));

        //return new BookDto
        //{
        //    Id = book.Id,
        //    Title = book.Title,
        //    Author = book.Author,
        //    ISBN = book.ISBN,
        //    Publisher = book.Publisher,
        //    PublicationYear = book.PublicationYear,
        //    Genre = book.Genre,
        //    Description = book.Description,
        //    TotalCopies = book.TotalCopies,
        //    AvailableCopies = book.AvailableCopies,
        //    ReservationCount = reservationCount
        //};
    }
}
