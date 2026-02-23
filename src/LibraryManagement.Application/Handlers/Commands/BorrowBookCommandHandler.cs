using LibraryManagement.Application.Commands.Books;
using LibraryManagement.Domain.Entities;
using LibraryManagement.Domain.Enums;
using LibraryManagement.Domain.Interfaces;
using MediatR;

namespace LibraryManagement.Application.Handlers.Commands;

public class BorrowBookCommandHandler : IRequestHandler<BorrowBookCommand, BorrowBookResult>
{
    private readonly IUnitOfWork _unitOfWork;
    private const int LoanDurationDays = 14;
    private const decimal FinePerDay = 50.00m;
    private const decimal MaxFineAmount = 1000.00m;

    public BorrowBookCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<BorrowBookResult> Handle(BorrowBookCommand request, CancellationToken cancellationToken)
    {
        // Provera da li korisnik postoji
        var user = await _unitOfWork.Users.FirstOrDefaultAsync(u => u.Id == request.UserId);
        if (user == null)
        {
            return new BorrowBookResult
            {
                Success = false,
                Message = "Korisnik ne postoji."
            };
        }

        // Provera da li je korisnik blokiran
        if (user.IsBlocked)
        {
            return new BorrowBookResult
            {
                Success = false,
                Message = "Vaš nalog je blokiran. Molimo kontaktirajte biblioteku."
            };
        }

        // Provera neplaćenih kazni
        if (user.TotalFines >= MaxFineAmount)
        {
            return new BorrowBookResult
            {
                Success = false,
                Message = $"Imate neplaćene kazne u iznosu od {user.TotalFines:C}. Molimo izmirite dug pre pozajmljivanja."
            };
        }

        // Provera da li knjiga postoji
        var book = await _unitOfWork.Books.GetByIdAsync(request.BookId);
        if (book == null)
        {
            return new BorrowBookResult
            {
                Success = false,
                Message = "Knjiga ne postoji."
            };
        }

        // Provera dostupnosti primerka
        if (book.AvailableCopies <= 0)
        {
            return new BorrowBookResult
            {
                Success = false,
                Message = "Nema dostupnih primeraka. Možete rezervisati knjigu."
            };
        }

        // Pronalaženje dostupnog primerka
        var availableCopy = (await _unitOfWork.BookCopies
            .FindAsync(bc => bc.BookId == request.BookId && bc.Status == BookCopyStatus.Available))
            .FirstOrDefault();

        if (availableCopy == null)
        {
            return new BorrowBookResult
            {
                Success = false,
                Message = "Greška: Sistem pokazuje dostupne primerke, ali nijedan nije pronađen."
            };
        }

        var copy = await _unitOfWork.BookCopies
            .FirstOrDefaultAsync(x => x.BookId == request.BookId && x.Status == BookCopyStatus.Available);

        if (copy == null)
            return new BorrowBookResult { Success = false, Message = "Nema dostupnih primeraka." };

        try
        {
            await _unitOfWork.BeginTransactionAsync();

            // Kreiranje pozajmice sa statusom PENDING
            var loan = new Loan
            {
                BookCopyId = copy.Id,
                UserId = request.UserId,
                LoanDate = DateTime.Now,
                DueDate = null, 
                Status = LoanStatus.Pending, 
                FineAmount = 0,
                FinePaid = false,
                CreatedAt = DateTime.Now
            };

            await _unitOfWork.Loans.AddAsync(loan);

            await _unitOfWork.SaveChangesAsync();
            await _unitOfWork.CommitTransactionAsync();

            return new BorrowBookResult
            {
                Success = true,
                Message = "Zahtev za pozajmicu je poslat. Čeka se odobrenje bibliotekara.",
                LoanId = loan.Id
            };
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackTransactionAsync();
            return new BorrowBookResult { Success = false, Message = ex.Message };
        }
    }
}