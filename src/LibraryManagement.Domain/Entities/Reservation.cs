using LibraryManagement.Domain.Enums;

namespace LibraryManagement.Domain.Entities;

public class Reservation
{
    public int Id { get; set; }
    public int BookId { get; set; }
    public string UserId { get; set; } = string.Empty;
    public DateTime ReservationDate { get; set; }
    public DateTime? ExpirationDate { get; set; }
    public ReservationStatus Status { get; set; }
    public int QueuePosition { get; set; }
    public DateTime? NotifiedDate { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    // Navigation properties
    public Book Book { get; set; } = null!;
    public ApplicationUser User { get; set; } = null!;
}
