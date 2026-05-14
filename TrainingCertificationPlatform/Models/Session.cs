// Represents scheduled training sessions including course, instructor, classroom, and session timing details.

using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace TrainingCertificationPlatform.Models
{
    public class Session
    {
        [Required]
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
