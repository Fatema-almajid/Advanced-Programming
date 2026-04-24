using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace MVC_Application.Models.ViewModels
{
    public class CertificationTrackFormViewModel
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Track Name")]
        public string Name { get; set; } = string.Empty;

        [Required]
        public string Description { get; set; } = string.Empty;

        [Display(Name = "Required Courses")]
        public List<int> SelectedCourseIds { get; set; } = new();

        public List<SelectListItem> CourseOptions { get; set; } = new();
    }
}