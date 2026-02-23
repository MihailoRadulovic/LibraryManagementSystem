using LibraryManagement.Domain.Enums;

namespace LibraryManagement.Domain.Entities;

public class BookCopy
{
    public int Id { get; set; }
    public int BookId { get; set; }
    public string CopyNumber { get; set; } = string.Empty;
    public BookCopyStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    // Navigation properties
    public Book Book { get; set; } = null!;
    public ICollection<Loan> Loans { get; set; } = new List<Loan>();
}
