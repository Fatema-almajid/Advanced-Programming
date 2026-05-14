// Represents classrooms and equipment with a many-to-many relationship for managing training room resources

namespace TrainingCertificationPlatform.Models
{
    public class Equipment
    {
        public int Id { get; set; }
        public string Name { get; set; } = String.Empty;
        public List<Classroom> Classrooms { get; set; } = new();
    }

    public class Classroom
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Seats { get; set; }

        // many-to-many relationship created using EFcore convention
        public List<Equipment> Equipments { get; set; } = new();
    }
}
