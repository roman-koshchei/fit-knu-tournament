using System.ComponentModel.DataAnnotations;

namespace Web.Models;
public class ChangePassword
{
    [Required(ErrorMessage = "The Old Password field is required")]
    [StringLength(18, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
    [DataType(DataType.Password)]
    [Display(Name = "Old Password")]
    public string OldPassword { get; set; }

    [Required(ErrorMessage = "The New Password field is required")]
    [StringLength(18, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
    [RegularExpression(@"^((?=.*[a-z])(?=.*[A-Z])(?=.*\d)).+$", ErrorMessage = "The New Password must contain at least one lowercase letter, one uppercase letter, and one digit.")]
    [DataType(DataType.Password)]
    [Display(Name = "New Password")]
    public string NewPassword { get; set; }

    [Required(ErrorMessage = "The Confirm Password field is required")]
    [Compare("NewPassword", ErrorMessage = "The New Password and Confirm Password do not match")]
    [DataType(DataType.Password)]
    [Display(Name = "Confirm Password")]
    public string ConfirmPassword { get; set; }
}