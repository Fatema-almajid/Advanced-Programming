using System.ComponentModel.DataAnnotations;

namespace TrainingCertificationPlatform.Models.Dto
{
    public class TraineeRegistrationDto
    {
        [StringLength(30)]
        public string FirstName { get; set; }
        [StringLength(30)]
        public string LastName { get; set; }
        [StringLength(100, MinimumLength = 4)]
        public string Password { get; set; }
        [EmailAddress]
        public string Email { get; set; }
        [StringLength(8, MinimumLength = 8)]
        public string Phone { get; set; }
    }
}
