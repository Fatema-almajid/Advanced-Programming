namespace TrainingCertificationPlatform.Models
{
    public enum AssessmentStatus
    {
        PENDING,
        COMPLETED
    }
    public class Assessment
    {
        public int Id { get; set; }
        public int EnrollmentId { get; set; }
        public AssessmentStatus Status { get; set; }
        public DateTime DueDate;
        public DateTime? CompletedBy;

        public Enrollment Enrollment { get; set; }
    }
}
