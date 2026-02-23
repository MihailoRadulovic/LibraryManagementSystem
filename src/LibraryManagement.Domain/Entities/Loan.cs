using LibraryManagement.Domain.Enums;

namespace LibraryManagement.Domain.Entities;

public class Loan
{
    public int Id { get; set; }
    public int BookCopyId { get; set; }
    public string UserId { get; set; } = string.Empty;
    public DateTime LoanDate { get; set; }
    public DateTime? DueDate { get; set; }
    public DateTime? ReturnDate { get; set; }
    public LoanStatus Status { get; set; }
    public decimal? FineAmount { get; set; }
    public bool FinePaid { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? ApprovedByUserId { get; set; }
    public DateTime? ApprovedAt { get; set; }
    public string? ReturnConfirmedByUserId { get; set; }
    public DateTime? ReturnConfirmedAt { get; set; }

    public virtual ApplicationUser? ApprovedBy { get; set; }
    public virtual ApplicationUser? ReturnConfirmedBy { get; set; }


    // Navigation properties
    public BookCopy BookCopy { get; set; } = null!;
    public ApplicationUser User { get; set; } = null!;
}
