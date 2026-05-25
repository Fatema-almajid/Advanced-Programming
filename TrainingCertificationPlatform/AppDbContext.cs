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
        public DbSet<Feedback> Feedbacks { get; set; }
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

            // Decimal Precision
            modelBuilder.Entity<Payment>()
                .Property(p => p.Amount)
                .HasPrecision(10, 2);

            modelBuilder.Entity<Balance>()
                .Property(b => b.AmountDue)
                .HasPrecision(10, 2);

            modelBuilder.Entity<Feedback>()
                .HasOne(f => f.Trainee)
                .WithMany(u => u.GivenFeedbacks)
                .HasForeignKey(f => f.TraineeId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Feedback>()
                .HasOne(f => f.Instructor)
                .WithMany(u => u.ReceivedFeedbacks)
                .HasForeignKey(f => f.InstructorId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Feedback>()
                .HasOne(f => f.Course)
                .WithMany(c => c.Feedbacks)
                .HasForeignKey(f => f.CourseId)
                .OnDelete(DeleteBehavior.Cascade);

            // SEED DATA

            var fixedDate = new DateTime(2026, 6, 15);

            // USERS

            modelBuilder.Entity<User>().HasData(

                new User
                {
                    Id = 1,
                    FirstName = "Ali",
                    LastName = "Ahmad",
                    CPR = "123456789",
                    Password = "$2a$12$Ys7YXxI9M7EqQY60T8aNFe31SwSs8IGXjfAYsFNp55NcGyzL4cIym",
                    Role = UserRole.TRAINEE,
                    Email = "ali@mail.com",
                    Phone = "99999991",
                    RegistrationDate = fixedDate
                },

                new User
                {
                    Id = 2,
                    FirstName = "Sara",
                    LastName = "Mohamed",
                    CPR = "987654321",
                    Password = "$2a$12$Ys7YXxI9M7EqQY60T8aNFe31SwSs8IGXjfAYsFNp55NcGyzL4cIym",
                    Role = UserRole.INSTRUCTOR,
                    Email = "sara@mail.com",
                    Phone = "99999992",
                    RegistrationDate = fixedDate
                },

                new User
                {
                    Id = 3,
                    FirstName = "Dana",
                    LastName = "Albanki",
                    CPR = "112233445",
                    Password = "$2a$12$Ys7YXxI9M7EqQY60T8aNFe31SwSs8IGXjfAYsFNp55NcGyzL4cIym",
                    Role = UserRole.TRAINING_COORDINATOR,
                    Email = "dana@mail.com",
                    Phone = "99999993",
                    RegistrationDate = fixedDate
                }
            );

            // COURSES

            modelBuilder.Entity<Course>().HasData(

                new Course
                {
                    Id = 1,
                    Category = CourseCategory.Programming,
                    Title = "C# Basics",
                    Description = "Introduction to C# programming",
                    Duration = 20,
                    Capacity = 10,
                    Fee = 120,
                    PrerequisiteId = null
                },

                new Course
                {
                    Id = 2,
                    Category = CourseCategory.Programming,
                    Title = "Advanced C#",
                    Description = "Advanced concepts in C#",
                    Duration = 30,
                    Capacity = 15,
                    Fee = 180,
                    PrerequisiteId = 1
                },

                new Course
                {
                    Id = 3,
                    Category = CourseCategory.Database,
                    Title = "SQL Fundamentals",
                    Description = "Introduction to SQL Server",
                    Duration = 25,
                    Capacity = 12,
                    Fee = 150,
                    PrerequisiteId = null
                }
            );

            // TRACKS

            modelBuilder.Entity<Track>().HasData(

                new Track
                {
                    Id = 1,
                    Name = "Backend Development",
                    Description = "Backend programming track"
                },

                new Track
                {
                    Id = 2,
                    Name = "Database Administration",
                    Description = "Database management track"
                }
            );

            // CLASSROOMS

            modelBuilder.Entity<Classroom>().HasData(

                new Classroom { Id = 1, Name = "Room A", Seats = 30 },
                new Classroom { Id = 2, Name = "Lab 1", Seats = 20 }
            );

            // EQUIPMENT

            modelBuilder.Entity<Equipment>().HasData(

                new Equipment { Id = 1, Name = "Projector" },
                new Equipment { Id = 2, Name = "Lab Computers" }
            );

            // SESSIONS

            modelBuilder.Entity<Session>().HasData(

                // NORMAL SESSION
                new Session
                {
                    Id = 1,
                    CourseId = 1,
                    InstructorId = 2,
                    ClassroomId = 1,
                    SessionDate = new DateTime(2026, 6, 22),
                    StartTime = new TimeOnly(9, 0),
                    EndTime = new TimeOnly(11, 0)
                },

                // CONFLICT SESSION (same instructor + same time)
                new Session
                {
                    Id = 2,
                    CourseId = 3,
                    InstructorId = 2,
                    ClassroomId = 2,
                    SessionDate = new DateTime(2026, 6, 22),
                    StartTime = new TimeOnly(9, 0),
                    EndTime = new TimeOnly(11, 0)
                },

                // ADVANCED C# AFTER PASSING BASICS
                new Session
                {
                    Id = 3,
                    CourseId = 2,
                    InstructorId = 2,
                    ClassroomId = 1,
                    SessionDate = new DateTime(2026, 6, 25),
                    StartTime = new TimeOnly(12, 0),
                    EndTime = new TimeOnly(14, 0)
                },

                // ANOTHER SESSION SAME DAY
                new Session
                {
                    Id = 4,
                    CourseId = 1,
                    InstructorId = 2,
                    ClassroomId = 2,
                    SessionDate = new DateTime(2026, 6, 25),
                    StartTime = new TimeOnly(10, 0),
                    EndTime = new TimeOnly(12, 0)
                }
            );

            // ENROLLMENTS

            modelBuilder.Entity<Enrollment>().HasData(

                // COMPLETED BASICS
                new Enrollment
                {
                    Id = 1,
                    TraineeId = 1,
                    SessionId = 1,
                    Status = EnrollmentStatus.COMPLETED,
                    EnrollmentDate = fixedDate,
                    CompletionDate = fixedDate.AddDays(7)
                },

                // ENROLLED IN ADVANCED AFTER COMPLETING BASICS
                new Enrollment
                {
                    Id = 2,
                    TraineeId = 1,
                    SessionId = 3,
                    Status = EnrollmentStatus.CONFIRMED,
                    EnrollmentDate = fixedDate.AddDays(8)
                },

                // DROPPED SQL COURSE
                new Enrollment
                {
                    Id = 3,
                    TraineeId = 1,
                    SessionId = 2,
                    Status = EnrollmentStatus.DROPPED,
                    EnrollmentDate = fixedDate
                }
            );

            // PAYMENTS

            modelBuilder.Entity<Payment>().HasData(

                new Payment
                {
                    Id = 1,
                    EnrollmentId = 1,
                    Amount = 120,
                    PaymentDate = fixedDate,
                    Status = PaymentStatus.FULL
                },

                new Payment
                {
                    Id = 2,
                    EnrollmentId = 2,
                    Amount = 180,
                    PaymentDate = fixedDate.AddDays(8),
                    Status = PaymentStatus.FULL
                },

                new Payment
                {
                    Id = 3,
                    EnrollmentId = 3,
                    Amount = 60,
                    PaymentDate = fixedDate,
                    Status = PaymentStatus.PARTIAL
                }
            );

            // BALANCES

            modelBuilder.Entity<Balance>().HasData(

                new Balance
                {
                    Id = 1,
                    EnrollmentId = 1,
                    AmountDue = 0,
                    DueDate = fixedDate
                },

                new Balance
                {
                    Id = 2,
                    EnrollmentId = 2,
                    AmountDue = 0,
                    DueDate = fixedDate.AddDays(8)
                },

                new Balance
                {
                    Id = 3,
                    EnrollmentId = 3,
                    AmountDue = 90,
                    DueDate = fixedDate.AddDays(7)
                }
            );

            // ASSESSMENTS

            modelBuilder.Entity<Assessment>().HasData(

                // PASSED BASICS
                new Assessment
                {
                    Id = 1,
                    EnrollmentId = 1,
                    Status = AssessmentStatus.PASS,
                    DueDate = fixedDate.AddDays(5),
                    CompletedBy = fixedDate.AddDays(7)
                }

            // NO ASSESSMENT FOR ACTIVE/DROPPED COURSES
            );

            // FEEDBACKS

            modelBuilder.Entity<Feedback>().HasData(

                // FEEDBACK ONLY FOR COMPLETED COURSE
                new Feedback
                {
                    Id = 1,
                    TraineeId = 1,
                    InstructorId = 2,
                    CourseId = 1,
                    Rating = 5,
                    Comment = "Excellent instructor and very clear explanations",
                    SubmittedAt = fixedDate.AddDays(7),
                    ContentRating = 5,
                    InstructorRating = 5,
                    OrganizationRating = 4,
                    RecommendCourse = true
                }
            );

            // NOTIFICATIONS

            modelBuilder.Entity<Notification>().HasData(

                new Notification
                {
                    Id = 1,
                    UserId = 1,
                    Message = "Welcome to the platform",
                    CreatedDate = fixedDate,
                    Status = NotificationStatus.UNREAD
                },

                new Notification
                {
                    Id = 2,
                    UserId = 2,
                    Message = "You have a scheduling conflict",
                    CreatedDate = fixedDate,
                    Status = NotificationStatus.UNREAD
                }
            );

            // INSTRUCTOR AVAILABILITY

            modelBuilder.Entity<InstructorAvailability>().HasData(

                new InstructorAvailability
                {
                    Id = 1,
                    InstructorId = 2,
                    DayStart = Day.SUNDAY,
                    DayEnd = Day.THURSDAY,
                    StartTime = new TimeOnly(8, 0),
                    EndTime = new TimeOnly(16, 0)
                }
            );

            // TRAINEE CERTIFICATIONS

            modelBuilder.Entity<TraineeCertification>().HasData(

                // ONLY AFTER COMPLETING TRACK
                new TraineeCertification
                {
                    Id = 1,
                    TraineeId = 1,
                    TrackId = 1,
                    CertificateReferenceNumber = "CERT-1001",
                    Status = TraineeCertificationStatus.SUCCESS
                }
            );

            // COURSE TRACK

            modelBuilder.Entity("CourseTrack").HasData(

                new { CoursesId = 1, TracksId = 1 },
                new { CoursesId = 2, TracksId = 1 },
                new { CoursesId = 3, TracksId = 2 }
            );

            // CLASSROOM EQUIPMENT

            modelBuilder.Entity("ClassroomEquipment").HasData(

                new { ClassroomsId = 1, EquipmentsId = 1 },
                new { ClassroomsId = 2, EquipmentsId = 2 }
            );

            // INSTRUCTOR EXPERTISE

            modelBuilder.Entity<InstructorExpertise>().HasData(

                new { InstructorId = 2, CourseId = 1 },
                new { InstructorId = 2, CourseId = 2 },
                new { InstructorId = 2, CourseId = 3 }
            );

        }
    }
}