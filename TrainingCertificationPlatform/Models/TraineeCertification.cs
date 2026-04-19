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

        public Track Track { get; set; }
        public User Trainee { get; set; }
    }
}
