namespace TrainingCertificationPlatform.Models
{
    public class Track
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

        // The courses for this track
        // Many-to-many relationship created via EF Core
        public List<Course> Courses { get; set; } = new();
    }
}
