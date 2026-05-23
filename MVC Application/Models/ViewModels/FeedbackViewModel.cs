using System.ComponentModel.DataAnnotations;

namespace MVC_Application.Models.ViewModels
{
    public class FeedbackViewModel
    {
        public int CourseId { get; set; }

        public int InstructorId { get; set; }

        [Range(1, 5)]
        public int Rating { get; set; }

        [Range(1, 5)]
        public int ContentRating { get; set; }

        [Range(1, 5)]
        public int InstructorRating { get; set; }

        [Range(1, 5)]
        public int OrganizationRating { get; set; }

        public bool RecommendCourse { get; set; }

        public string? Comment { get; set; }
    }
}