using System.ComponentModel.DataAnnotations.Schema;

namespace TrainingCertificationPlatform.Models
{
    public enum TraineeCertificationStatus
    {
        FAILED,
        SUCCESS
    }
    public class TraineeCertification
    {
        public int Id { get; set; }
        public int TraineeId { get; set; }
        public int TrackId { get; set; }
        public TraineeCertificationStatus Status { get; set; }

        [ForeignKey("TraineeId")]
        public User Trainee { get; set; } = null!;

        [ForeignKey("TrackId")]
        public Track Track { get; set; } = null!;
    }
}
