using LibraryManagement.Domain.Entities;

namespace LibraryManagement.Domain.Interfaces;

public interface IUnitOfWork : IDisposable
{
    IRepository<Book> Books { get; }
    IRepository<BookCopy> BookCopies { get; }
    IRepository<Loan> Loans { get; }
    IRepository<Reservation> Reservations { get; }
    IRepository<ApplicationUser> Users { get; }
    
    Task<int> SaveChangesAsync();
    Task BeginTransactionAsync();
    Task CommitTransactionAsync();
    Task RollbackTransactionAsync();
}
