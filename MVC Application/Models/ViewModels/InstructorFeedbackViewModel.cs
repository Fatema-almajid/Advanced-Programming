using System;

namespace MVC_Application.Models.ViewModels
{
    public class InstructorFeedbackViewModel
    {
        public string CourseTitle { get; set; }

        public string TraineeName { get; set; }

        public int Rating { get; set; }

        public int ContentRating { get; set; }

        public int InstructorRating { get; set; }

        public int OrganizationRating { get; set; }

        public bool RecommendCourse { get; set; }

        public string Comment { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}