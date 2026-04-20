using System.ComponentModel.DataAnnotations;

namespace TrainingCertificationPlatform.Models.Dto
{
    public class TraineeRegistrationDto
    {
        [StringLength(30)]
        public string FirstName { get; set; } = string.Empty;
        [StringLength(30)]
        public string LastName { get; set; } = string.Empty;
        [StringLength(100, MinimumLength = 4)]
        public string Password { get; set; } = string.Empty;
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
        [StringLength(8, MinimumLength = 8)]
        public string Phone { get; set; } = string.Empty;
    }
}
