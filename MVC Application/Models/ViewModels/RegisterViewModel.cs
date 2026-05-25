using System.ComponentModel.DataAnnotations;

namespace MVC_Application.Models.ViewModels
{
    public class RegisterViewModel
    {
        [Required]
        [StringLength(50, MinimumLength = 2)]
        [Display(Name = "First Name")]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [StringLength(50, MinimumLength = 2)]
        [Display(Name = "Last Name")]
        public string LastName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [RegularExpression(@"^\d{8}$", ErrorMessage = "Phone number must contain exactly 8 digits.")]
        public string Phone { get; set; } = string.Empty;

        [Required]
        [RegularExpression(@"^\d{9}$", ErrorMessage = "CPR must contain exactly 9 digits.")]
        public string CPR { get; set; } = string.Empty;

        [Required]
        [StringLength(100, MinimumLength = 8, ErrorMessage = "Password must be at least 8 characters long.")]
        [RegularExpression(
            @"^(?=.*[a-z])(?=.*[A-Z])(?=.*[^a-zA-Z0-9]).{8,}$",
            ErrorMessage = "Password must be at least 8 characters long and contain at least one uppercase letter, one lowercase letter, and one special character."
        )]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Password)]
        [Compare(nameof(Password), ErrorMessage = "Password and confirmation password do not match.")]
        [Display(Name = "Confirm Password")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}