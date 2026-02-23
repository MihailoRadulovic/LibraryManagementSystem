using LibraryManagement.Application.Commands.Loans;
using LibraryManagement.Domain.Enums;
using LibraryManagement.Domain.Interfaces;
using MediatR;

namespace LibraryManagement.Application.Handlers.Commands;

public class ApproveLoanCommandHandler : IRequestHandler<ApproveLoanCommand, ApproveLoanResult>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IEmailService _emailService;

    public ApproveLoanCommandHandler(IUnitOfWork unitOfWork, IEmailService emailService)
    {
        _unitOfWork = unitOfWork;
        _emailService = emailService;
    }

    public async Task<ApproveLoanResult> Handle(ApproveLoanCommand request, CancellationToken cancellationToken)
    {
        // 1. Pronađi pozajmicu
        var loan = await _unitOfWork.Loans.GetByIdAsync(request.LoanId);
        if (loan == null)
            return new ApproveLoanResult { Success = false, Message = "Pozajmica ne postoji." };

        if (loan.Status != LoanStatus.Pending)
            return new ApproveLoanResult { Success = false, Message = "Pozajmica nije na čekanju." };

        // 2. Pronađi primerak, knjigu i korisnika
        var copy = await _unitOfWork.BookCopies.GetByIdAsync(loan.BookCopyId);
        var book = await _unitOfWork.Books.GetByIdAsync(copy.BookId);
        var user = await _unitOfWork.Users.FirstOrDefaultAsync(u => u.Id == loan.UserId);

        try
        {
            await _unitOfWork.BeginTransactionAsync();

            if (request.Approve)
            {
                // ODOBRENO
                loan.Status = LoanStatus.Approved;
                loan.ApprovedByUserId = request.LibrarianUserId;
                loan.ApprovedAt = DateTime.Now;
                loan.DueDate = DateTime.Now.AddDays(14);  // Rok: 14 dana

                // SADA menjaj status primerka
                copy.Status = BookCopyStatus.Borrowed;
                _unitOfWork.BookCopies.Update(copy);

                // SADA smanjuj AvailableCopies
                book.AvailableCopies--;
                _unitOfWork.Books.Update(book);

                _unitOfWork.Loans.Update(loan);

                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitTransactionAsync();

                // Pošalji email obaveštenje o odobrenju
                if (user?.Email != null)
                {
                    try
                    {
                        await _emailService.SendEmailAsync(
                            user.Email,
                            "Pozajmica odobrena - Biblioteka",
                            $@"<h2>Poštovani {user.FirstName} {user.LastName},</h2>
                            <p>Vaša pozajmica je <strong>odobrena</strong>.</p>
                            <p><strong>Knjiga:</strong> {book.Title} - {book.Author}</p>
                            <p><strong>Rok za vraćanje:</strong> {loan.DueDate:dd.MM.yyyy}</p>
                            <p>Možete preuzeti knjigu u biblioteci.</p>
                            <br/>
                            <p>Srdačan pozdrav,<br/>Vaša biblioteka</p>");
                    }
                    catch
                    {
                        // Email greška ne treba da spreči odobrenje pozajmice
                    }
                }

                return new ApproveLoanResult
                {
                    Success = true,
                    Message = "Pozajmica je odobrena. Korisnik može preuzeti knjigu."
                };
            }
            else
            {
                // ODBIJENO
                loan.Status = LoanStatus.Rejected;
                loan.ApprovedByUserId = request.LibrarianUserId;
                loan.ApprovedAt = DateTime.Now;

                _unitOfWork.Loans.Update(loan);

                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitTransactionAsync();

                // Pošalji email obaveštenje o odbijanju
                if (user?.Email != null)
                {
                    try
                    {
                        await _emailService.SendEmailAsync(
                            user.Email,
                            "Pozajmica odbijena - Biblioteka",
                            $@"<h2>Poštovani {user.FirstName} {user.LastName},</h2>
                            <p>Nažalost, vaš zahtev za pozajmicu je <strong>odbijen</strong>.</p>
                            <p><strong>Knjiga:</strong> {book.Title} - {book.Author}</p>
                            <p>Za više informacija, obratite se bibliotekaru.</p>
                            <br/>
                            <p>Srdačan pozdrav,<br/>Vaša biblioteka</p>");
                    }
                    catch
                    {
                        // Email greška ne treba da spreči odbijanje pozajmice
                    }
                }

                return new ApproveLoanResult
                {
                    Success = true,
                    Message = "Pozajmica je odbijena."
                };
            }
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackTransactionAsync();
            return new ApproveLoanResult { Success = false, Message = ex.Message };
        }
    }
}