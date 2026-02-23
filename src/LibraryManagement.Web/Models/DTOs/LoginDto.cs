using System.ComponentModel.DataAnnotations;

namespace LibraryManagement.Web.Models.DTOs;

public class LoginDto
{
    [Required(ErrorMessage = "Email je obavezan.")]
    [EmailAddress(ErrorMessage = "Nevalidan format email adrese.")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Lozinka je obavezna.")]
    public string Password { get; set; } = string.Empty;
}
