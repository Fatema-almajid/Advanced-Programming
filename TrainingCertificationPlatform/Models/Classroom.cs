namespace TrainingCertificationPlatform.Models
{
    public class Equipment
    {
        public int Id { get; set; }
        public string Name { get; set; } = String.Empty;
        public List<Classroom> Classrooms { get; set; }
    }

    public class Classroom
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Seats { get; set; }

        // many-to-many relationship created using EFcore convention
        public List<Equipment> Equipments { get; set; }
    }
}
