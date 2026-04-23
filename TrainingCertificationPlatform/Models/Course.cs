namespace TrainingCertificationPlatform.Models
{
    public enum CourseCategory
    {
        None = 0,
        Programming = 1,
        Database = 2,
        WebDevelopment = 3,
        Networking = 4,
        Cybersecurity = 5,
        CloudComputing = 6
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

        public List<Track> Tracks { get; set; } = new();
        public List<InstructorExpertise> InstructorExpertises { get; set; } = new();

    }
}
