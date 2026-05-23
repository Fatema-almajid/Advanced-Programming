using System.ComponentModel.DataAnnotations;

namespace TrainingCertificationPlatform.Models
{
    public class Feedback
    {
        public int Id { get; set; }

        public int TraineeId { get; set; }
        public User? Trainee { get; set; }

        public int InstructorId { get; set; }
        public User? Instructor { get; set; }

        public int CourseId { get; set; }
        public Course? Course { get; set; }

        [Range(1, 5)]
        public int Rating { get; set; }

        [MaxLength(500)]
        public string? Comment { get; set; }

        public DateTime SubmittedAt { get; set; } = DateTime.Now;
        public int ContentRating { get; set; }

        public int InstructorRating { get; set; }

        public int OrganizationRating { get; set; }

        public bool RecommendCourse { get; set; }
    }
}