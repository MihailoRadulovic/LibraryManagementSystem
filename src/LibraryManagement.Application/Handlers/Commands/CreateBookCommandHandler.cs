using LibraryManagement.Application.Commands.Books;
using LibraryManagement.Domain.Entities;
using LibraryManagement.Domain.Enums;
using LibraryManagement.Domain.Interfaces;
using MediatR;

namespace LibraryManagement.Application.Handlers.Commands;

public class CreateBookCommandHandler : IRequestHandler<CreateBookCommand, int>
{
    private readonly IUnitOfWork _unitOfWork;

    public CreateBookCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<int> Handle(CreateBookCommand request, CancellationToken cancellationToken)
    {
        // Provera da li već postoji knjiga sa istim ISBN
        var existingBook = await _unitOfWork.Books.FirstOrDefaultAsync(b => b.ISBN == request.ISBN);
        if (existingBook != null)
        {
            throw new InvalidOperationException("Knjiga sa ovim ISBN-om već postoji.");
        }

        try
        {
            await _unitOfWork.BeginTransactionAsync();

            var book = new Book
            {
                Title = request.Title,
                Author = request.Author,
                ISBN = request.ISBN,
                Publisher = request.Publisher,
                PublicationYear = request.PublicationYear,
                Genre = request.Genre,
                Description = request.Description,
                TotalCopies = request.TotalCopies,
                AvailableCopies = request.TotalCopies,
                CreatedAt = DateTime.Now
            };

            await _unitOfWork.Books.AddAsync(book);
            await _unitOfWork.SaveChangesAsync();

            // Kreiranje primeraka knjige
            var bookCopies = new List<BookCopy>();
            for (int i = 1; i <= request.TotalCopies; i++)
            {
                bookCopies.Add(new BookCopy
                {
                    BookId = book.Id,
                    CopyNumber = $"{book.ISBN}-{i:D3}",
                    Status = BookCopyStatus.Available,
                    CreatedAt = DateTime.Now
                });
            }

            await _unitOfWork.BookCopies.AddRangeAsync(bookCopies);
            await _unitOfWork.SaveChangesAsync();
            await _unitOfWork.CommitTransactionAsync();

            return book.Id;
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync();
            throw;
        }
    }
}
