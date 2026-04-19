namespace TrainingCertificationPlatform.Models
{
    public enum NotificationStatus
    {
        UNREAD,
        READ
    }

    public class Notification
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
        public string Message { get; set; } = String.Empty;
        public DateTime CreatedDate { get; set; }
        public NotificationStatus Status { get; set; }
    }
}
