using System.ComponentModel.DataAnnotations;

namespace LibraryManagement.Web.Models.DTOs;

public class RegisterDto
{
    [Required(ErrorMessage = "Email je obavezan.")]
    [EmailAddress(ErrorMessage = "Nevalidan format email adrese.")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Lozinka je obavezna.")]
    [MinLength(6, ErrorMessage = "Lozinka mora imati najmanje 6 karaktera.")]
    public string Password { get; set; } = string.Empty;

    [Required(ErrorMessage = "Potvrda lozinke je obavezna.")]
    [Compare("Password", ErrorMessage = "Lozinke se ne poklapaju.")]
    public string ConfirmPassword { get; set; } = string.Empty;

    [Required(ErrorMessage = "Ime je obavezno.")]
    public string FirstName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Prezime je obavezno.")]
    public string LastName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Datum rođenja je obavezan.")]
    public DateTime DateOfBirth { get; set; }

    public string Address { get; set; } = string.Empty;

    [Phone(ErrorMessage = "Nevalidan format broja telefona.")]
    public string PhoneNumber { get; set; } = string.Empty;
}
