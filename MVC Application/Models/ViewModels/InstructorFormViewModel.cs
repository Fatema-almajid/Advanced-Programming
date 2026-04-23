using System.ComponentModel.DataAnnotations;
using TrainingCertificationPlatform.Models;

namespace MVC_Application.Models.ViewModels
{
    public class InstructorFormViewModel : IValidatableObject
    {
        public int Id { get; set; }

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
        [StringLength(100)]
        public string Email { get; set; } = string.Empty;

        [Required]
        [RegularExpression(@"^\d{8}$", ErrorMessage = "Phone number must contain exactly 8 digits.")]
        public string Phone { get; set; } = string.Empty;

        [Display(Name = "Registration Date")]
        [DataType(DataType.Date)]
        public DateTime RegistrationDate { get; set; } = DateTime.Today;

        [Display(Name = "Password")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters.")]
        [DataType(DataType.Password)]
        public string? Password { get; set; }

        [Display(Name = "Confirm Password")]
        [DataType(DataType.Password)]
        [Compare(nameof(Password), ErrorMessage = "Password and confirmation password do not match.")]
        public string? ConfirmPassword { get; set; }

        [Display(Name = "Available From Day")]
        public Day DayStart { get; set; } = Day.SUNDAY;

        [Display(Name = "Available Until Day")]
        public Day DayEnd { get; set; } = Day.THURSDAY;

        [Display(Name = "Start Time")]
        [DataType(DataType.Time)]
        public TimeOnly StartTime { get; set; } = new(9, 0);

        [Display(Name = "End Time")]
        [DataType(DataType.Time)]
        public TimeOnly EndTime { get; set; } = new(17, 0);

        [Display(Name = "Expertise Courses")]
        public List<int> SelectedCourseIds { get; set; } = new();

        public bool IsEditMode => Id > 0;

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (!IsEditMode && string.IsNullOrWhiteSpace(Password))
            {
                yield return new ValidationResult("Password is required when creating an instructor.", new[] { nameof(Password) });
            }

            if (RegistrationDate.Date > DateTime.Today)
            {
                yield return new ValidationResult("Registration date cannot be in the future.", new[] { nameof(RegistrationDate) });
            }

            if (EndTime <= StartTime)
            {
                yield return new ValidationResult("End time must be later than start time.", new[] { nameof(EndTime) });
            }

            if (DayEnd < DayStart)
            {
                yield return new ValidationResult("Available until day cannot be earlier than available from day.", new[] { nameof(DayEnd) });
            }
        }
    }
}