using System.ComponentModel.DataAnnotations.Schema;

namespace TrainingCertificationPlatform.Models
{
    public enum Day
    {
        SUNDAY,
        MONDAY,
        TUESDAY,
        WEDNESDAY,
        THURSDAY,
        FRIDAY,
        SATURDAY
    }
    public class InstructorAvailability
    {
        public int Id { get; set; }
        public int InstructorId { get; set; }
        public Day DayStart { get; set; }
        public Day DayEnd { get; set; }
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }

        [ForeignKey("InstructorId")]
        public User Instructor { get; set; }

        // Instructor available from DayStart - DayEnd during times StartTime - EndTime
    }
}
