using LibraryManagement.Application.Commands.Books;
using LibraryManagement.Domain.Enums;
using LibraryManagement.Domain.Interfaces;
using MediatR;

namespace LibraryManagement.Application.Handlers.Commands;

public class ReturnBookCommandHandler : IRequestHandler<ReturnBookCommand, ReturnBookResult>
{
    private readonly IUnitOfWork _unitOfWork;
    private const decimal FinePerDay = 50.00m;

    public ReturnBookCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ReturnBookResult> Handle(ReturnBookCommand request, CancellationToken cancellationToken)
    {
        // Pronalaženje pozajmice
        var loan = await _unitOfWork.Loans.GetByIdAsync(request.LoanId);


        if (loan == null)
            return new ReturnBookResult { Success = false, Message = "Pozajmica ne postoji." };

        if (loan.Status == LoanStatus.Returned)
            return new ReturnBookResult { Success = false, Message = "Knjiga je već vraćena." };

        if (loan.Status == LoanStatus.PendingReturn)
            return new ReturnBookResult { Success = false, Message = "Vraćanje je već prijavljeno i čeka potvrdu." };


        loan.ReturnDate = DateTime.Now;
        loan.Status = LoanStatus.PendingReturn;  // ← NOVO!
        loan.UpdatedAt = DateTime.Now;

        _unitOfWork.Loans.Update(loan);
        await _unitOfWork.SaveChangesAsync();

        return new ReturnBookResult
        {
            Success = true,
            Message = "Vraćanje prijavljeno. Čeka se potvrda bibliotekara."
        };
    }
}
