using LibraryManagement.Application.Queries.Loans;
using LibraryManagement.Domain.Enums;
using LibraryManagement.Domain.Interfaces;
using MediatR;

namespace LibraryManagement.Application.Handlers.Queries;

public class GetPendingLoansQueryHandler : IRequestHandler<GetPendingLoansQuery, List<PendingLoanDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetPendingLoansQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<List<PendingLoanDto>> Handle(GetPendingLoansQuery request, CancellationToken cancellationToken)
    {
        // 1. Pronađi sve pozajmice sa statusom Pending
        var pendingLoans = await _unitOfWork.Loans.FindAsync(l => l.Status == LoanStatus.Pending);

        var result = new List<PendingLoanDto>();

        foreach (var loan in pendingLoans)
        {
            // 2. Za svaku pozajmicu, učitaj BookCopy, Book i User
            var bookCopy = await _unitOfWork.BookCopies.GetByIdAsync(loan.BookCopyId);
            if (bookCopy == null) continue;

            var book = await _unitOfWork.Books.GetByIdAsync(bookCopy.BookId);
            if (book == null) continue;

            var user = await _unitOfWork.Users.FirstOrDefaultAsync(u => u.Id == loan.UserId);
            if (user == null) continue;

            // 3. Kreiraj DTO
            result.Add(new PendingLoanDto
            {
                LoanId = loan.Id,
                BookTitle = book.Title,
                BookAuthor = book.Author,
                UserName = $"{user.FirstName} {user.LastName}",
                UserEmail = user.Email ?? "",
                RequestDate = loan.LoanDate
            });
        }

        return result.OrderBy(x => x.RequestDate).ToList();
    }
}