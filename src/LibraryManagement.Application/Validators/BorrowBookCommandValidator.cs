using FluentValidation;
using LibraryManagement.Application.Commands.Books;

namespace LibraryManagement.Application.Validators;

public class BorrowBookCommandValidator : AbstractValidator<BorrowBookCommand>
{
    public BorrowBookCommandValidator()
    {
        RuleFor(x => x.BookId)
            .GreaterThan(0)
            .WithMessage("ID knjige mora biti veći od 0.");

        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage("ID korisnika je obavezan.");
    }
}
