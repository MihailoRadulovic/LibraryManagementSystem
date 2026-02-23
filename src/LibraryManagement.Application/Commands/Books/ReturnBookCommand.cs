using MediatR;

namespace LibraryManagement.Application.Commands.Books;

public class ReturnBookCommand : IRequest<ReturnBookResult>
{
    public int LoanId { get; set; }
}

public class ReturnBookResult
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public decimal? FineAmount { get; set; }
    public int? DaysOverdue { get; set; }
}
