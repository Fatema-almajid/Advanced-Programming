namespace TrainingCertificationPlatform.Models
{
    public enum AssessmentStatus
    {
        PENDING,
        PASS,
        FAIL
    }
    public class Assessment
    {
        public int Id { get; set; }
        public int EnrollmentId { get; set; }
        public AssessmentStatus Status { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime? CompletedBy { get; set; }

        public Enrollment Enrollment { get; set; }
    }
}
