using TrainingCertificationPlatform.Models;

namespace MVC_Application.Models.ViewModels
{
    public class InstructorDashboardViewModel
    {
        public int AssignedCourses { get; set; }
        public int UpcomingSessions { get; set; }
        public int PastSessions { get; set; }
        public int PendingAssessments { get; set; }

        public List<InstructorSessionViewModel> UpcomingSessionList { get; set; } = new();
    }

    public class InstructorSessionViewModel
    {
        public int SessionId { get; set; }
        public int CourseId { get; set; }

        public string CourseTitle { get; set; } = string.Empty;
        public string CourseDescription { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string ClassroomName { get; set; } = string.Empty;

        public DateTime SessionDate { get; set; }
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }

        public int Capacity { get; set; }
        public int EnrolledCount { get; set; }
        public int PendingAssessments { get; set; }

        public string SessionType { get; set; } = string.Empty;
    }

    public class InstructorCourseDetailsViewModel
    {
        public int SessionId { get; set; }

        public string CourseTitle { get; set; } = string.Empty;
        public string CourseDescription { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string ClassroomName { get; set; } = string.Empty;

        public DateTime SessionDate { get; set; }
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }

        public List<InstructorTraineeAssessmentViewModel> Trainees { get; set; } = new();
    }

    public class InstructorTraineeAssessmentViewModel
    {
        public int EnrollmentId { get; set; }

        public string TraineeName { get; set; } = string.Empty;
        public string TraineeEmail { get; set; } = string.Empty;

        public EnrollmentStatus EnrollmentStatus { get; set; }
        public AssessmentStatus AssessmentStatus { get; set; }
    }

    public class InstructorScheduleViewModel
    {
        public List<InstructorSessionViewModel> UpcomingSessions { get; set; } = new();
        public List<InstructorSessionViewModel> PastSessions { get; set; } = new();
        public List<InstructorAvailability> Availabilities { get; set; } = new();
    }
}