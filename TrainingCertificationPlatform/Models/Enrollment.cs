using System.ComponentModel.DataAnnotations.Schema;

namespace TrainingCertificationPlatform.Models
{
    public enum EnrollmentStatus
    {
        PENDING,
        APPROVED,
        REJECTED
    }
    public class Enrollment
    {
        public int Id { get; set; }
        public int TraineeId { get; set; }
        public int SessionId { get; set; }
        public EnrollmentStatus Status { get; set; }
        public DateTime EnrollmentDate;
        public DateTime? CompletionDate;
        public DateTime? PaymentDueDate;

        [ForeignKey("TraineeId")]
        public User Trainee;
        public Session Session;
    }
}
