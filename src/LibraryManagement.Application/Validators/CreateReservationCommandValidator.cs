using FluentValidation;
using LibraryManagement.Application.Commands.Reservations;

namespace LibraryManagement.Application.Validators;

public class CreateReservationCommandValidator : AbstractValidator<CreateReservationCommand>
{
    public CreateReservationCommandValidator()
    {
        RuleFor(x => x.BookId)
            .GreaterThan(0)
            .WithMessage("ID knjige mora biti veći od 0.");

        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage("ID korisnika je obavezan.");
    }
}
