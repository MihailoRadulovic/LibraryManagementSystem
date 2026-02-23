using MediatR;

namespace LibraryManagement.Application.Commands.Loans;

public class ConfirmReturnCommand : IRequest<ConfirmReturnResult>
{
    public int LoanId { get; set; }
    public string LibrarianUserId { get; set; } = string.Empty;
}

public class ConfirmReturnResult
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public decimal FineAmount { get; set; }
}