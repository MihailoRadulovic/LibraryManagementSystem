using LibraryManagement.Application.Queries.Loans;
using LibraryManagement.Domain.Enums;
using LibraryManagement.Domain.Interfaces;
using MediatR;

namespace LibraryManagement.Application.Handlers.Queries;

public class GetPendingReturnsQueryHandler : IRequestHandler<GetPendingReturnsQuery, List<PendingReturnDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetPendingReturnsQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<List<PendingReturnDto>> Handle(GetPendingReturnsQuery request, CancellationToken cancellationToken)
    {
        var pendingReturns = await _unitOfWork.Loans.FindAsync(l => l.Status == LoanStatus.PendingReturn);

        var result = new List<PendingReturnDto>();

        foreach (var loan in pendingReturns)
        {
            var bookCopy = await _unitOfWork.BookCopies.GetByIdAsync(loan.BookCopyId);
            if (bookCopy == null) continue;

            var book = await _unitOfWork.Books.GetByIdAsync(bookCopy.BookId);
            if (book == null) continue;

            var user = await _unitOfWork.Users.FirstOrDefaultAsync(u => u.Id == loan.UserId);
            if (user == null) continue;

            var isOverdue = loan.DueDate.HasValue && loan.ReturnDate > loan.DueDate.Value;
            var daysOverdue = isOverdue
                ? (loan.ReturnDate.Value - loan.DueDate.Value).Days
                : 0;

            result.Add(new PendingReturnDto
            {
                LoanId = loan.Id,
                BookTitle = book.Title,
                BookAuthor = book.Author,
                UserName = $"{user.FirstName} {user.LastName}",
                UserEmail = user.Email ?? "",
                BorrowDate = loan.LoanDate,
                DueDate = loan.DueDate,
                ReturnDate = loan.ReturnDate ?? DateTime.Now,
                IsOverdue = isOverdue,
                DaysOverdue = daysOverdue
            });
        }

        return result.OrderBy(x => x.ReturnDate).ToList();
    }
}