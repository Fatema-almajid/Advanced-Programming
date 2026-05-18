namespace MVC_Application.Models.ViewModels
{
    public class TraineeDashboardViewModel
    {
        public int CoursesEnrolled { get; set; }
        public int CoursesCompleted { get; set; }
        public int ActivitiesCompleted { get; set; }
        public int ActivitiesDue { get; set; }

        public List<TraineeCourseViewModel> MyCourses { get; set; } = new();
    }

    public class TraineeCourseViewModel
    {
        public int EnrollmentId { get; set; }
        public int CourseId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime SessionDate { get; set; }
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
    }

    public class TraineeCertificationProgressViewModel
    {
        public int TrackId { get; set; }
        public string TrackName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int RequiredCourses { get; set; }
        public int CompletedCourses { get; set; }
        public int RemainingCourses { get; set; }
        public int ProgressPercent { get; set; }
        public bool IsEligible { get; set; }

        public string? CertificateReferenceNumber { get; set; }
        public List<CertificationCourseItemViewModel> Courses { get; set; } = new();
    }

    public class CertificationCourseItemViewModel
    {
        public string CourseTitle { get; set; } = string.Empty;
        public bool IsCompleted { get; set; }
        public string Status { get; set; }
    }
}