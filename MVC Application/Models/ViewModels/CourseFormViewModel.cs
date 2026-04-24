using System.ComponentModel.DataAnnotations;
using TrainingCertificationPlatform.Models;

namespace MVC_Application.Models.ViewModels
{
    public class CourseFormViewModel
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 3)]
        [Display(Name = "Course Title")]
        public string Title { get; set; } = string.Empty;

        [Required]
        [StringLength(500, MinimumLength = 5)]
        public string Description { get; set; } = string.Empty;

        [Required]
        public CourseCategory Category { get; set; }

        [Range(1, 365, ErrorMessage = "Duration must be between 1 and 365 hours.")]
        [Display(Name = "Duration (Hours)")]
        public int Duration { get; set; }

        [Range(1, 500, ErrorMessage = "Capacity must be between 1 and 500.")]
        public int Capacity { get; set; }

        [Range(9.9, 1000, ErrorMessage = "Fee must be greater than 9BD and less than 1000BD.")]
        [Display(Name = "Course Fee")]
        [DisplayFormat(DataFormatString = "{0:N2}", ApplyFormatInEditMode = true)]
        public double Fee { get; set; }

        [Display(Name = "Prerequisite Course")]
        public int? PrerequisiteId { get; set; }
    }
}