using MediatR;

namespace LibraryManagement.Application.Queries.Loans;

public class GetUserLoansQuery : IRequest<UserLoansViewModel>
{
    public string UserId { get; set; } = string.Empty;
}

public class UserLoansViewModel
{
    public List<UserLoanDto> PendingRequests { get; set; } = new();
    public List<UserLoanDto> ApprovedLoans { get; set; } = new();
    public List<UserLoanDto> RejectedRequests { get; set; } = new();
    public List<UserLoanDto> ReturnedLoans { get; set; } = new();
}

public class UserLoanDto
{
    public int LoanId { get; set; }
    public string BookTitle { get; set; } = string.Empty;
    public string BookAuthor { get; set; } = string.Empty;
    public DateTime RequestDate { get; set; }
    public DateTime? ApprovedDate { get; set; }
    public DateTime? DueDate { get; set; }
    public DateTime? ReturnDate { get; set; }
    public string Status { get; set; } = string.Empty;
    public string StatusBadge { get; set; } = string.Empty;
    public bool CanReturn { get; set; }
}