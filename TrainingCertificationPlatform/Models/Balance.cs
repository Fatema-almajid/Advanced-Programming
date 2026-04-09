using System.ComponentModel.DataAnnotations.Schema;

namespace TrainingCertificationPlatform.Models
{
    public class Balance
    {
        public int Id { get; set; }
        public int TraineeId { get; set; }

        // The amount the Trainee currently owes to the platform
        public int AmountDue { get; set; }
        public DateTime DueDate { get; set; }

        [ForeignKey("TraineeId")]
        public User Trainee;
    }
}
