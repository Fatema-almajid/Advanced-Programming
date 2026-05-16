using System.ComponentModel.DataAnnotations.Schema;
// Represents trainee certifications linked to training tracks, including certification status and trainee-track relationships

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

        // for public certification lookup
        public string CertificateReferenceNumber { get; set; } = string.Empty;

        [ForeignKey("TraineeId")]
        public User Trainee { get; set; } = null!;

        [ForeignKey("TrackId")]
        public Track Track { get; set; } = null!;
    }
}
