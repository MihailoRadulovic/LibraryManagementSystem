using LibraryManagement.Domain.Enums;

namespace LibraryManagement.Application.DTOs;

public class ReservationDto
{
    public int Id { get; set; }
    public int BookId { get; set; }
    public string BookTitle { get; set; } = string.Empty;
    public string BookAuthor { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public DateTime ReservationDate { get; set; }
    public DateTime? ExpirationDate { get; set; }
    public ReservationStatus Status { get; set; }
    public int QueuePosition { get; set; }
    public DateTime? NotifiedDate { get; set; }
}
