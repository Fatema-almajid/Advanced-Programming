using System.ComponentModel.DataAnnotations.Schema;

namespace TrainingCertificationPlatform.Models
{
    public class Session
    {
        public int Id { get; set; }
        public int CourseId { get; set; }

        [ForeignKey("CourseId")]
        public Course Course { get; set; } = null!;
        public int InstructorId { get; set; }

        public int ClassroomId { get; set; }

        public DateTime SessionDate { get; set; }
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }

        [ForeignKey("InstructorId")]
        public User Instructor { get; set; } = null!;

        [ForeignKey("ClassroomId")]
        public Classroom Classroom { get; set; } = null!;
    }
}
