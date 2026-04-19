namespace TrainingCertificationPlatform.Models
{
    public enum CourseCategory
    {
        None = 0 // TODO
    }

    public class Course
    {
        public int Id { get; set; }
        public CourseCategory Category { get; set; }
        public string Title { get; set; } = String.Empty;
        public string Description { get; set; } = String.Empty;
        public int? PrerequisiteId { get; set; }
        public Course? Prerequisite { get; set; }
        public int Duration { get; set; } // credits?
        public int Capacity { get; set; }
        public double Fee { get; set; }

        public List<Track> Tracks { get; set; }
    }
}
