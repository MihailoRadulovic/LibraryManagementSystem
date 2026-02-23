using LibraryManagement.Application.DTOs;
using MediatR;

namespace LibraryManagement.Application.Queries.Books;

public class GetAllBooksQuery : IRequest<IEnumerable<BookDto>>
{
    public string? SearchTerm { get; set; }
    public string? Genre { get; set; }
    public bool? OnlyAvailable { get; set; }
}
