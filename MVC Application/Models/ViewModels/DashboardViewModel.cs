namespace MVC_Application.Models.ViewModels
{
    public class DashboardViewModel
    {
        public int TotalTrainees { get; set; }
        public int TotalInstructors { get; set; }
        public int TotalCourses { get; set; }
        public int ActiveSchedules { get; set; }
        public int PendingPayments { get; set; }
        public int CertificatesIssued { get; set; }

        public List<UpcomingCourseViewModel> UpcomingCourses { get; set; } = new();
        public List<RecentEnrollmentViewModel> RecentEnrollments { get; set; } = new();
    }
}