// Represents trainee payment records linked to enrollments, including amount, payment date, and payment status

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

        //REEM: Changed to decimal
        public decimal Amount { get; set; }

        public DateTime PaymentDate { get; set; }

        public PaymentStatus Status { get; set; }

        public Enrollment Enrollment { get; set; } = null!;
    }
}