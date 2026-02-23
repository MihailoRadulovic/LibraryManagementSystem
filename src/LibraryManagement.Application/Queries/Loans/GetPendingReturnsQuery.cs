using LibraryManagement.Application.DTOs;
using MediatR;

namespace LibraryManagement.Application.Queries.Loans;

public class GetPendingReturnsQuery : IRequest<List<PendingReturnDto>>
{
    // Nema parametara
}

public class PendingReturnDto
{
    public int LoanId { get; set; }
    public string BookTitle { get; set; } = string.Empty;
    public string BookAuthor { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string UserEmail { get; set; } = string.Empty;
    public DateTime BorrowDate { get; set; }
    public DateTime? DueDate { get; set; }
    public DateTime ReturnDate { get; set; }
    public bool IsOverdue { get; set; }
    public int DaysOverdue { get; set; }
}