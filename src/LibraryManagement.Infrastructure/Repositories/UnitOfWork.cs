using LibraryManagement.Domain.Entities;
using LibraryManagement.Domain.Interfaces;
using LibraryManagement.Infrastructure.Data;
using Microsoft.EntityFrameworkCore.Storage;

namespace LibraryManagement.Infrastructure.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _context;
    private IDbContextTransaction? _transaction;

    public UnitOfWork(ApplicationDbContext context)
    {
        _context = context;
        Books = new Repository<Book>(_context);
        BookCopies = new Repository<BookCopy>(_context);
        Loans = new Repository<Loan>(_context);
        Reservations = new Repository<Reservation>(_context);
        Users = new Repository<ApplicationUser>(_context);
    }

    public IRepository<Book> Books { get; private set; }
    public IRepository<BookCopy> BookCopies { get; private set; }
    public IRepository<Loan> Loans { get; private set; }
    public IRepository<Reservation> Reservations { get; private set; }
    public IRepository<ApplicationUser> Users { get; private set; }

    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }

    public async Task BeginTransactionAsync()
    {
        _transaction = await _context.Database.BeginTransactionAsync();
    }

    public async Task CommitTransactionAsync()
    {
        if (_transaction != null)
        {
            await _transaction.CommitAsync();
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public async Task RollbackTransactionAsync()
    {
        if (_transaction != null)
        {
            await _transaction.RollbackAsync();
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public void Dispose()
    {
        _transaction?.Dispose();
        _context.Dispose();
    }
}
