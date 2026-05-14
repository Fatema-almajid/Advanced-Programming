// Represents the many-to-many relationship between instructors and courses based on instructor expertise

using TrainingCertificationPlatform.Models;

namespace TrainingCertificationPlatform.Models
{
    public class InstructorExpertise
    {
        public int InstructorId { get; set; }
        public int CourseId { get; set; }

        public User Instructor { get; set; } = null!;
        public Course Course { get; set; } = null!;
    }
}