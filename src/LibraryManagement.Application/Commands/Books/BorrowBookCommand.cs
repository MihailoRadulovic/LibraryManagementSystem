using MediatR;

namespace LibraryManagement.Application.Commands.Books;

public class BorrowBookCommand : IRequest<BorrowBookResult>
{
    public int BookId { get; set; }
    public string UserId { get; set; } = string.Empty;
}

public class BorrowBookResult
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public int? LoanId { get; set; }
    public DateTime? DueDate { get; set; }
}
