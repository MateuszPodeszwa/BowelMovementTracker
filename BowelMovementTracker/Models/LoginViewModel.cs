using System.ComponentModel.DataAnnotations;
using BowelMovementTracker.Data;

namespace BowelMovementTracker.Models;

public class LoginViewModel
{
    public Guid? UserIdentifier { get; init; }
    public Diary? Diary { get; init; }
    
    [Required(ErrorMessage = "An email address is required.")]
    [EmailAddress(ErrorMessage = "Please enter a valid email address.")]
    [Display(Name = "Email Address")]
    public required string UserEmailAddress { get; init; }

    [Required(ErrorMessage = "A password is required.")]
    [DataType(DataType.Password)]
    [Display(Name = "Password")]
    public required string UserPasswordHash { get; init; }

    [Display(Name = "Remember me?")]
    public bool? RememberMe { get; set; }
}