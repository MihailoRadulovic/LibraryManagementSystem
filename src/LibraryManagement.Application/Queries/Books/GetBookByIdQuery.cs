using LibraryManagement.Application.DTOs;
using MediatR;

namespace LibraryManagement.Application.Queries.Books;

public class GetBookByIdQuery : IRequest<BookDto?>
{
    public int Id { get; set; }
}
