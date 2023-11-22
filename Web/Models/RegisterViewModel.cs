using System.ComponentModel.DataAnnotations;

namespace Web.Models
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "The Email field is required")]
        [EmailAddress(ErrorMessage = "Incorrect e-mail address")]
        public string Email { get; set; }

        [Required(ErrorMessage = "The Password field is required")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Passwords do not match")]
        public string ConfirmPassword { get; set; }
    }
}
