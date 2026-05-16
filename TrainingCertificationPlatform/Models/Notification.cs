// Represents notifications sent to users, including message details, creation date, and read status

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
        public User User { get; set; } = null!;

        public string Message { get; set; } = string.Empty;

        public DateTime CreatedDate { get; set; } = DateTime.Now;

        public NotificationStatus Status { get; set; } = NotificationStatus.UNREAD;
    }
}