using System.ComponentModel.DataAnnotations.Schema;

namespace TrainingCertificationPlatform.Models
{
    public class Balance
    {
        public int Id { get; set; }

        public int EnrollmentId { get; set; }

        public int AmountDue { get; set; }
        public DateTime DueDate { get; set; }

        [ForeignKey("EnrollmentId")]
        public Enrollment Enrollment { get; set; }
    }
}