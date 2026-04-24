using System.ComponentModel.DataAnnotations;

namespace MVC_Application.Models.ViewModels
{
    public class SessionFormViewModel
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Course")]
        public int CourseId { get; set; }

        [Required]
        [Display(Name = "Instructor")]
        public int InstructorId { get; set; }

        [Required]
        [Display(Name = "Room")]
        public int ClassroomId { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Session Date")]
        public DateTime SessionDate { get; set; }

        [Required]
        [DataType(DataType.Time)]
        [Display(Name = "Start Time")]
        public TimeOnly StartTime { get; set; }

        [Required]
        [DataType(DataType.Time)]
        [Display(Name = "End Time")]
        public TimeOnly EndTime { get; set; }
    }
}