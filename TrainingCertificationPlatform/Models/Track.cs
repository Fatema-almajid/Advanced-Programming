namespace TrainingCertificationPlatform.Models
{
    public class Track
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        // The courses for this track
        // Many-to-many relationship created via EF Core
        public List<Course> Courses { get; set; }
    }
}
