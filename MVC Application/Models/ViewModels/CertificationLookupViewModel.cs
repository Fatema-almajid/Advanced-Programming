// for public certification lookup

namespace MVC_Application.Models.ViewModels
{
    public class CertificationLookupViewModel
    {
        public int TraineeId { get; set; }

        public string ReferenceNumber { get; set; } = string.Empty;

        public string? TraineeName { get; set; }

        public string? Track { get; set; }

        public string? Status { get; set; }

        public List<string> CompletedCourses { get; set; } = new();
    }
}