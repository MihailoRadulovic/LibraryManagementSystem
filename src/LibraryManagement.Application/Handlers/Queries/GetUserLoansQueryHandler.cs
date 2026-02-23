using LibraryManagement.Application.Queries.Loans;
using LibraryManagement.Domain.Enums;
using LibraryManagement.Domain.Interfaces;
using MediatR;

namespace LibraryManagement.Application.Handlers.Queries;

public class GetUserLoansQueryHandler : IRequestHandler<GetUserLoansQuery, UserLoansViewModel>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetUserLoansQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<UserLoansViewModel> Handle(GetUserLoansQuery request, CancellationToken cancellationToken)
    {
        // Pronađi sve pozajmice korisnika
        var allLoans = await _unitOfWork.Loans.FindAsync(l => l.UserId == request.UserId);

        var viewModel = new UserLoansViewModel();

        foreach (var loan in allLoans)
        {
            var bookCopy = await _unitOfWork.BookCopies.GetByIdAsync(loan.BookCopyId);
            if (bookCopy == null) continue;

            var book = await _unitOfWork.Books.GetByIdAsync(bookCopy.BookId);
            if (book == null) continue;

            var dto = new UserLoanDto
            {
                LoanId = loan.Id,
                BookTitle = book.Title,
                BookAuthor = book.Author,
                RequestDate = loan.LoanDate,
                ApprovedDate = loan.ApprovedAt,
                DueDate = loan.DueDate,
                ReturnDate = loan.ReturnDate,
                Status = GetStatusText(loan.Status),
                StatusBadge = GetStatusBadge(loan.Status),
                CanReturn = loan.Status == LoanStatus.Approved
            };

            // Kategorisanje po statusu
            switch (loan.Status)
            {
                case LoanStatus.Pending:
                    viewModel.PendingRequests.Add(dto);
                    break;
                case LoanStatus.Approved:
                    viewModel.ApprovedLoans.Add(dto);
                    break;
                case LoanStatus.Rejected:
                    viewModel.RejectedRequests.Add(dto);
                    break;
                case LoanStatus.Returned:
                case LoanStatus.PendingReturn:
                    viewModel.ReturnedLoans.Add(dto);
                    break;
            }
        }

        return viewModel;
    }

    private string GetStatusText(LoanStatus status)
    {
        return status switch
        {
            LoanStatus.Pending => "Čeka odobrenje",
            LoanStatus.Approved => "Odobreno - Možete preuzeti",
            LoanStatus.Rejected => "Odbijeno",
            LoanStatus.PendingReturn => "Čeka se potvrda vraćanja",
            LoanStatus.Returned => "Vraćeno",
            _ => "Nepoznat status"
        };
    }

    private string GetStatusBadge(LoanStatus status)
    {
        return status switch
        {
            LoanStatus.Pending => "warning",
            LoanStatus.Approved => "success",
            LoanStatus.Rejected => "danger",
            LoanStatus.PendingReturn => "info",
            LoanStatus.Returned => "secondary",
            _ => "secondary"
        };
    }
}