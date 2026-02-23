using MediatR;

namespace LibraryManagement.Application.Commands.Loans;

public class ApproveLoanCommand : IRequest<ApproveLoanResult>
{
    public int LoanId { get; set; }
    public string LibrarianUserId { get; set; } = string.Empty;
    public bool Approve { get; set; }  
}

public class ApproveLoanResult
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
}