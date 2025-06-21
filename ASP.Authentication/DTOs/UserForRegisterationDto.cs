using System.ComponentModel.DataAnnotations;
using ASP.Authentication.Data;

namespace ASP.Authentication.DTOs;

public class UserForRegisterationDto
{
    [Required(ErrorMessage = "Name is Required.")]
    public string? Name { get; set; }

    [Required]
    [EmailAddress]
    public string? Email { get; set; }

    [Required(ErrorMessage = "Gender is Required.")]
    public Gender Gender { get; set; }

    [Required(ErrorMessage = "The password is Required.")]
    public string? Password { get; set; }

    [Required]
    [Compare("Password", ErrorMessage = "Password and Confirm password do not match.")]
    public string? ConfirmPassword { get; set; }
}
