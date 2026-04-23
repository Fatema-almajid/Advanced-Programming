using System.ComponentModel.DataAnnotations;

namespace MVC_Application.Models.ViewModels
{
    public class ClassroomFormViewModel
    {
        public int Id { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 2)]
        [Display(Name = "Room Name")]
        public string Name { get; set; } = string.Empty;

        [Required]
        [Range(1, 500, ErrorMessage = "Seats must be between 1 and 500.")]
        [Display(Name = "Seating Capacity")]
        public int Seats { get; set; }

        [Display(Name = "Assigned Equipment")]
        public List<int> SelectedEquipmentIds { get; set; } = new();
    }
}