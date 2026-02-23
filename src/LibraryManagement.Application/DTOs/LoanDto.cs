using LibraryManagement.Domain.Enums;

namespace LibraryManagement.Application.DTOs;

public class LoanDto
{
    public int Id { get; set; }
    public int BookCopyId { get; set; }
    public string BookTitle { get; set; } = string.Empty;
    public string BookAuthor { get; set; } = string.Empty;
    public string CopyNumber { get; set; } = string.Empty;
    public DateTime LoanDate { get; set; }
    public DateTime? DueDate { get; set; }
    public DateTime? ReturnDate { get; set; }
    public LoanStatus Status { get; set; }
    public decimal? FineAmount { get; set; }
    public bool FinePaid { get; set; }
    public int? DaysOverdue { get; set; }
}
