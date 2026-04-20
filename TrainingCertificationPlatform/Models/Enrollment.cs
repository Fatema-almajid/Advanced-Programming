using System.ComponentModel.DataAnnotations.Schema;

namespace TrainingCertificationPlatform.Models
{
    public enum EnrollmentStatus
    {
        ENROLLED,
        CONFIRMED,
        ATTENDING,
        COMPLETED,
        DROPPED
    }
    public class Enrollment
    {
        public int Id { get; set; }
        public int TraineeId { get; set; }
        public int SessionId { get; set; }
        public EnrollmentStatus Status { get; set; }
        public DateTime EnrollmentDate { get; set; }

        public DateTime? CompletionDate { get; set; }
        public DateTime? PaymentDueDate { get; set; }

        [ForeignKey("TraineeId")]
        public User Trainee { get; set; } = null!;

        [ForeignKey("SessionId")]
        public Session Session { get; set; } = null!;
    }
}
