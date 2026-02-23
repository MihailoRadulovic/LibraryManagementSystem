using FluentValidation;
using LibraryManagement.Application.Commands.Books;

namespace LibraryManagement.Application.Validators;

public class CreateBookCommandValidator : AbstractValidator<CreateBookCommand>
{
    public CreateBookCommandValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Naslov je obavezan.")
            .MaximumLength(200).WithMessage("Naslov ne može biti duži od 200 karaktera.");

        RuleFor(x => x.Author)
            .NotEmpty().WithMessage("Autor je obavezan.")
            .MaximumLength(200).WithMessage("Ime autora ne može biti duže od 200 karaktera.");

        RuleFor(x => x.ISBN)
            .NotEmpty().WithMessage("ISBN je obavezan.")
            .Matches(@"^(?:\d{10}|\d{13})$").WithMessage("ISBN mora imati 10 ili 13 cifara.");

        RuleFor(x => x.Publisher)
            .MaximumLength(200).WithMessage("Naziv izdavača ne može biti duži od 200 karaktera.");

        RuleFor(x => x.PublicationYear)
            .InclusiveBetween(1000, DateTime.Now.Year + 1)
            .WithMessage($"Godina izdanja mora biti između 1000 i {DateTime.Now.Year + 1}.");

        RuleFor(x => x.Genre)
            .MaximumLength(100).WithMessage("Žanr ne može biti duži od 100 karaktera.");

        RuleFor(x => x.Description)
            .MaximumLength(1000).WithMessage("Opis ne može biti duži od 1000 karaktera.");

        RuleFor(x => x.TotalCopies)
            .GreaterThan(0).WithMessage("Broj primeraka mora biti veći od 0.")
            .LessThanOrEqualTo(1000).WithMessage("Broj primeraka ne može biti veći od 1000.");
    }
}
