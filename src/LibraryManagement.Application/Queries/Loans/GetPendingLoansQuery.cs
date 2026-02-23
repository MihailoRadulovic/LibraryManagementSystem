using LibraryManagement.Application.DTOs;
using MediatR;

namespace LibraryManagement.Application.Queries.Loans;

public class GetPendingLoansQuery : IRequest<List<PendingLoanDto>>
{
    // Nema parametara - vraća sve pending pozajmice
}

public class PendingLoanDto
{
    public int LoanId { get; set; }
    public string BookTitle { get; set; } = string.Empty;
    public string BookAuthor { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string UserEmail { get; set; } = string.Empty;
    public DateTime RequestDate { get; set; }
}