using System.ComponentModel.DataAnnotations;

namespace LibraryManagement.Web.Models.ViewModels;

public class RegisterViewModel
{
    [Required(ErrorMessage = "Email je obavezan.")]
    [EmailAddress(ErrorMessage = "Nevalidan format email adrese.")]
    [Display(Name = "Email")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Lozinka je obavezna.")]
    [StringLength(100, ErrorMessage = "{0} mora imati najmanje {2} karaktera.", MinimumLength = 6)]
    [DataType(DataType.Password)]
    [Display(Name = "Lozinka")]
    public string Password { get; set; } = string.Empty;

    [DataType(DataType.Password)]
    [Display(Name = "Potvrdi lozinku")]
    [Compare("Password", ErrorMessage = "Lozinke se ne poklapaju.")]
    public string ConfirmPassword { get; set; } = string.Empty;

    [Required(ErrorMessage = "Ime je obavezno.")]
    [Display(Name = "Ime")]
    public string FirstName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Prezime je obavezno.")]
    [Display(Name = "Prezime")]
    public string LastName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Datum rođenja je obavezan.")]
    [DataType(DataType.Date)]
    [Display(Name = "Datum rođenja")]
    public DateTime DateOfBirth { get; set; }

    [Display(Name = "Adresa")]
    public string Address { get; set; } = string.Empty;

    [Phone(ErrorMessage = "Nevalidan format broja telefona.")]
    [Display(Name = "Broj telefona")]
    public string PhoneNumber { get; set; } = string.Empty;
}
