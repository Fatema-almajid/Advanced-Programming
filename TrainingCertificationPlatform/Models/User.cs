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
        public string Username { get; set; }
        public UserRole Role { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public DateTime RegistrationDate { get; set; }
    }
}
