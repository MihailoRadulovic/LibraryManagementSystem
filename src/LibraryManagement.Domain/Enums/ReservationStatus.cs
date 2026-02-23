namespace LibraryManagement.Domain.Enums;

public enum ReservationStatus
{
    Pending = 0,    // Čeka u redu
    Ready = 1,      // Spremna za preuzimanje
    Expired = 2,    // Istekla (nije preuzeta na vreme)
    Cancelled = 3,  // Otkazana od strane korisnika
    PickedUp = 4    // ← DODAJ OVO! Preuzeta (kreirana pozajmica)
}