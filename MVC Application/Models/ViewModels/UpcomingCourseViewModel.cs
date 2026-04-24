namespace MVC_Application.Models.ViewModels
{
    public class UpcomingCourseViewModel
    {
        public string CourseName { get; set; }
        public string InstructorName { get; set; }
        public string RoomName { get; set; }
        public string ScheduleDate { get; set; }
        public string ScheduleTime { get; set; }
        public int Capacity { get; set; }
        public int EnrolledCount { get; set; }
    }
}