namespace TrainingCertificationPlatform.Models
{
    public enum UserRole
    {
        TRAINEE,
        INSTRUCTOR,
        TRAINING_COORDINATOR
    }

    public class User
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public UserRole Role { get; set; }
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public DateTime RegistrationDate { get; set; }
        public List<InstructorExpertise> InstructorExpertises { get; set; } = new();
        public List<Enrollment> Enrollments { get; set; } = new();
        public List<Notification> Notifications { get; set; } = new();
    }
}
