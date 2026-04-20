namespace TrainingCertificationPlatform.Models
{
    public enum PaymentStatus
    {
        PARTIAL,
        FULL
    }

    public class Payment
    {
        public int Id { get; set; }

        public int EnrollmentId { get; set; }

        public double Amount { get; set; }

        public DateTime PaymentDate { get; set; }

        public PaymentStatus Status { get; set; }

        public Enrollment Enrollment { get; set; } = null!;
    }
}