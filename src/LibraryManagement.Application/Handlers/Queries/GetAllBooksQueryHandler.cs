using LibraryManagement.Application.DTOs;
using LibraryManagement.Application.Queries.Books;
using LibraryManagement.Domain.Interfaces;
using MediatR;

namespace LibraryManagement.Application.Handlers.Queries;

public class GetAllBooksQueryHandler : IRequestHandler<GetAllBooksQuery, IEnumerable<BookDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetAllBooksQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<BookDto>> Handle(GetAllBooksQuery request, CancellationToken cancellationToken)
    {
        var books = await _unitOfWork.Books.GetAllAsync();

        // Filtriranje
        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            books = books.Where(b => 
                b.Title.Contains(request.SearchTerm, StringComparison.OrdinalIgnoreCase) ||
                b.Author.Contains(request.SearchTerm, StringComparison.OrdinalIgnoreCase) ||
                b.ISBN.Contains(request.SearchTerm, StringComparison.OrdinalIgnoreCase));
        }

        if (!string.IsNullOrWhiteSpace(request.Genre))
        {
            books = books.Where(b => b.Genre.Equals(request.Genre, StringComparison.OrdinalIgnoreCase));
        }

        if (request.OnlyAvailable == true)
        {
            books = books.Where(b => b.AvailableCopies > 0);
        }

        var bookDtos = new List<BookDto>();
        foreach (var book in books)
        {
            var reservationCount = await _unitOfWork.Reservations.CountAsync(
                r => r.BookId == book.Id && 
                     (r.Status == Domain.Enums.ReservationStatus.Pending || 
                      r.Status == Domain.Enums.ReservationStatus.Ready));

            bookDtos.Add(new BookDto
            {
                Id = book.Id,
                Title = book.Title,
                Author = book.Author,
                ISBN = book.ISBN,
                Publisher = book.Publisher,
                PublicationYear = book.PublicationYear,
                Genre = book.Genre,
                Description = book.Description,
                TotalCopies = book.TotalCopies,
                AvailableCopies = book.AvailableCopies,
                ReservationCount = reservationCount
            });
        }

        return bookDtos.OrderBy(b => b.Title);
    }
}
