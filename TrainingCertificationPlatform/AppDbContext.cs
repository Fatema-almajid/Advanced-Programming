using Microsoft.EntityFrameworkCore;
using TrainingCertificationPlatform.Models;

namespace TrainingCertificationPlatform
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<Classroom> Classrooms { get; set; }
        public DbSet<Equipment> Equipments { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<Session> Sessions { get; set; }
        public DbSet<Enrollment> Enrollments { get; set; }
        public DbSet<Balance> Balances { get; set; }

        public DbSet<Payment> Payments { get; set; }
        public DbSet<InstructorAvailability> InstructorAvailabilities { get; set; }
        public DbSet<TraineeCertification> TraineeCertifications { get; set; }
        public DbSet<Track> Tracks { get; set; }
        public DbSet<Assessment> Assessments { get; set; }
        public DbSet<InstructorExpertise> InstructorExpertises { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // InstructorExpertise 
            modelBuilder.Entity<InstructorExpertise>()
                .HasKey(e => new { e.InstructorId, e.CourseId });

            modelBuilder.Entity<InstructorExpertise>()
                .HasOne(e => e.Instructor)
                .WithMany(u => u.InstructorExpertises)
                .HasForeignKey(e => e.InstructorId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<InstructorExpertise>()
                .HasOne(e => e.Course)
                .WithMany(c => c.InstructorExpertises)
                .HasForeignKey(e => e.CourseId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Enrollment>()
                .HasOne(e => e.Trainee)
                .WithMany()
                .HasForeignKey(e => e.TraineeId)
                .OnDelete(DeleteBehavior.Restrict);

            //REEM: create one-to-one relationship between Enrollment and Balance
            modelBuilder.Entity<Enrollment>()
   .HasOne(e => e.Balance)
   .WithOne(b => b.Enrollment)
   .HasForeignKey<Balance>(b => b.EnrollmentId);

            modelBuilder.Entity<Session>()
                .HasOne(s => s.Instructor)
                .WithMany()
                .HasForeignKey(s => s.InstructorId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Session>()
                .HasIndex(s => new { s.InstructorId, s.SessionDate });

            modelBuilder.Entity<Session>()
                .HasIndex(s => new { s.ClassroomId, s.SessionDate });

            //REEM: .WithMany() to .WithMany(e => e.Payments)
            modelBuilder.Entity<Payment>()
               .HasOne(p => p.Enrollment)
               .WithMany(e => e.Payments)
               .HasForeignKey(p => p.EnrollmentId);

            modelBuilder.Entity<Assessment>()
                .HasOne(a => a.Enrollment)
                .WithOne()
                .HasForeignKey<Assessment>(a => a.EnrollmentId);

            // SeedData (Data to test)

            var fixedDate = new DateTime(2026, 4, 4);

            // USER
            modelBuilder.Entity<User>().HasData(
                new User { Id = 1, FirstName = "Ali", LastName = "Ahmad", Password = "$2a$11$examplehash...", Role = UserRole.TRAINEE, Email = "ali@mail.com", Phone = "99999999", RegistrationDate = fixedDate },
                new User { Id = 2, FirstName = "Sara", LastName = "Mohamed", Password = "$2a$11$examplehash...", Role = UserRole.INSTRUCTOR, Email = "sara@mail.com", Phone = "88888888", RegistrationDate = fixedDate },
                //REEM: Add a training coordinator user for testing
                new User { Id = 100, FirstName = "Mariam", LastName = "Coordinator", Password = "123456", Role = UserRole.TRAINING_COORDINATOR, Email = "coordinator@mail.com", Phone = "77777777", RegistrationDate = fixedDate}
                );

            // COURSE
            modelBuilder.Entity<Course>().HasData(
                new Course { Id = 1, Category = CourseCategory.Database, Title = "C# Basics", Description = "Intro", Duration = 10, Capacity = 30, Fee = 100 },
                new Course { Id = 2, Category = CourseCategory.Programming, Title = "Advanced C#", Description = "Advanced", Duration = 15, Capacity = 25, Fee = 150, PrerequisiteId = 1 }
            );

            // TRACK
            modelBuilder.Entity<Track>().HasData(
                new Track { Id = 1, Name = "Backend", Description = "Programming Track" }
            );

            // CLASSROOM
            modelBuilder.Entity<Classroom>().HasData(
                new Classroom { Id = 1, Name = "Room A", Seats = 30 }
            );

            // EQUIPMENT
            modelBuilder.Entity<Equipment>().HasData(
                new Equipment { Id = 1, Name = "Projector" }
            );

            // SESSION
            modelBuilder.Entity<Session>().HasData(
                new Session { Id = 1, CourseId = 1, InstructorId = 2, ClassroomId = 1, SessionDate = fixedDate, StartTime = new TimeOnly(10, 0), EndTime = new TimeOnly(12, 0) }
            );

            // ENROLLMENT
            modelBuilder.Entity<Enrollment>().HasData(
                new Enrollment { Id = 1, TraineeId = 1, SessionId = 1, Status = EnrollmentStatus.ENROLLED, EnrollmentDate = fixedDate }
            );

            // PAYMENT
            modelBuilder.Entity<Payment>().HasData(
                new Payment { Id = 1, EnrollmentId = 1, Amount = 100, PaymentDate = fixedDate, Status = PaymentStatus.FULL }
            );

            // BALANCE
            modelBuilder.Entity<Balance>().HasData(
                new Balance { Id = 1, EnrollmentId = 1, AmountDue = 50, DueDate = fixedDate }
            );

            // ASSESSMENT
            modelBuilder.Entity<Assessment>().HasData(
                new Assessment { Id = 1, EnrollmentId = 1, Status = AssessmentStatus.PENDING, DueDate = fixedDate }
            );

            // NOTIFICATION
            modelBuilder.Entity<Notification>().HasData(
                new Notification { Id = 1, UserId = 1, Message = "Welcome", CreatedDate = fixedDate, Status = NotificationStatus.UNREAD }
            );

            // INSTRUCTOR AVAILABILITY
            modelBuilder.Entity<InstructorAvailability>().HasData(
                new InstructorAvailability { Id = 1, InstructorId = 2, DayStart = Day.SUNDAY, DayEnd = Day.THURSDAY, StartTime = new TimeOnly(9, 0), EndTime = new TimeOnly(17, 0) }
            );

            // TRAINEE CERTIFICATION
            modelBuilder.Entity<TraineeCertification>().HasData(
                new TraineeCertification { Id = 1, TraineeId = 1, TrackId = 1, Status = TraineeCertificationStatus.SUCCESS }
            );

            // CourseTrack
            modelBuilder.Entity("CourseTrack").HasData(
                new { CoursesId = 1, TracksId = 1 },
                new { CoursesId = 2, TracksId = 1 }
            );

            // ClassroomEquipment
            modelBuilder.Entity("ClassroomEquipment").HasData(
                new { ClassroomsId = 1, EquipmentsId = 1 }
            );

            // InstructorExpertise
            modelBuilder.Entity<InstructorExpertise>().HasData(
                new { InstructorId = 2, CourseId = 1 },
                new { InstructorId = 2, CourseId = 2 }
            );
        }

    }
}
