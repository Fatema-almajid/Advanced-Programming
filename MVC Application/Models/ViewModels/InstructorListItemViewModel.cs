namespace MVC_Application.Models.ViewModels
{
    public class InstructorListItemViewModel
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public DateTime RegistrationDate { get; set; }
        public string Availability { get; set; } = "Not set";
        public string ExpertiseCourses { get; set; } = "None";
    }
}