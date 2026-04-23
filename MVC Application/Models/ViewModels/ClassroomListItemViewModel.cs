namespace MVC_Application.Models.ViewModels
{
    public class ClassroomListItemViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Seats { get; set; }
        public string EquipmentNames { get; set; } = "None";
    }
}