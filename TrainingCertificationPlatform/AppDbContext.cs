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

            // Decimal Precision
            modelBuilder.Entity<Payment>()
                .Property(p => p.Amount)
                .HasPrecision(10, 2);

            modelBuilder.Entity<Balance>()
                .Property(b => b.AmountDue)
                .HasPrecision(10, 2);

            // SeedData (Data to test)

            var fixedDate = new DateTime(2026, 4, 4);

            // USER
            modelBuilder.Entity<User>().HasData(
                new User
                {
                    Id = 1,
                    FirstName = "Ali",
                    LastName = "Ahmad",
                    Password = "$2a$12$ZPzIhfjkDv3uc/4fEkhAfuAM/hYixvISLMEhyBYk7dxrsGJdw15Rq",
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
                    Password = "$2a$12$ZPzIhfjkDv3uc/4fEkhAfuAM/hYixvISLMEhyBYk7dxrsGJdw15Rq",
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
                    Password = "$2a$12$ZPzIhfjkDv3uc/4fEkhAfuAM/hYixvISLMEhyBYk7dxrsGJdw15Rq",
                    Role = UserRole.TRAINING_COORDINATOR,
                    Email = "dana@mail.com",
                    Phone = "99999993",
                    RegistrationDate = fixedDate
                },
                new User
                {
                    Id = 4,
                    FirstName = "Omar",
                    LastName = "Ali",
                    Password = "$2a$12$ZPzIhfjkDv3uc/4fEkhAfuAM/hYixvISLMEhyBYk7dxrsGJdw15Rq",
                    Role = UserRole.TRAINEE,
                    Email = "omar@mail.com",
                    Phone = "99999994",
                    RegistrationDate = fixedDate
                },
                new User
                {
                    Id = 5,
                    FirstName = "Fatima",
                    LastName = "Yousef",
                    Password = "$2a$12$ZPzIhfjkDv3uc/4fEkhAfuAM/hYixvISLMEhyBYk7dxrsGJdw15Rq",
                    Role = UserRole.INSTRUCTOR,
                    Email = "fatima@mail.com",
                    Phone = "99999995",
                    RegistrationDate = fixedDate
                }
            );

            // COURSE
            modelBuilder.Entity<Course>().HasData(
                new Course
                {
                    Id = 1,
                    Category = CourseCategory.Programming,
                    Title = "C# Basics",
                    Description = "Introduction to C# programming",
                    Duration = 20,
                    Capacity = 25,
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
                    Capacity = 20,
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
                    Capacity = 30,
                    Fee = 150,
                    PrerequisiteId = null
                },
                new Course
                {
                    Id = 4,
                    Category = CourseCategory.Database,
                    Title = "Entity Framework Core",
                    Description = "Working with EF Core",
                    Duration = 20,
                    Capacity = 20,
                    Fee = 170,
                    PrerequisiteId = 3
                },
                new Course
                {
                    Id = 5,
                    Category = CourseCategory.Networking,
                    Title = "Networking Basics",
                    Description = "Introduction to networking",
                    Duration = 15,
                    Capacity = 25,
                    Fee = 100,
                    PrerequisiteId = null
                }
            );

            // TRACK
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
                },
                new Track
                {
                    Id = 3,
                    Name = "Networking Essentials",
                    Description = "Networking certification track"
                },
                new Track
                {
                    Id = 4,
                    Name = "Full Stack Development",
                    Description = "Complete web development track"
                },
                new Track
                {
                    Id = 5,
                    Name = "Software Engineering",
                    Description = "Software engineering foundations"
                }
            );

            // CLASSROOM
            modelBuilder.Entity<Classroom>().HasData(
                new Classroom { Id = 1, Name = "Room A", Seats = 30 },
                new Classroom { Id = 2, Name = "Room B", Seats = 25 },
                new Classroom { Id = 3, Name = "Lab 1", Seats = 20 },
                new Classroom { Id = 4, Name = "Lab 2", Seats = 20 },
                new Classroom { Id = 5, Name = "Conference Hall", Seats = 50 }
            );

            // EQUIPMENT
            modelBuilder.Entity<Equipment>().HasData(
                new Equipment { Id = 1, Name = "Projector" },
                new Equipment { Id = 2, Name = "Whiteboard" },
                new Equipment { Id = 3, Name = "Lab Computers" },
                new Equipment { Id = 4, Name = "Microphones" },
                new Equipment { Id = 5, Name = "Networking Kit" }
            );

            // SESSION
            modelBuilder.Entity<Session>().HasData(
                new Session
                {
                    Id = 1,
                    CourseId = 1,
                    InstructorId = 2,
                    ClassroomId = 1,
                    SessionDate = fixedDate,
                    StartTime = new TimeOnly(9, 0),
                    EndTime = new TimeOnly(11, 0)
                },
                new Session
                {
                    Id = 2,
                    CourseId = 2,
                    InstructorId = 2,
                    ClassroomId = 2,
                    SessionDate = fixedDate.AddDays(1),
                    StartTime = new TimeOnly(12, 0),
                    EndTime = new TimeOnly(14, 0)
                },
                new Session
                {
                    Id = 3,
                    CourseId = 3,
                    InstructorId = 5,
                    ClassroomId = 3,
                    SessionDate = fixedDate.AddDays(2),
                    StartTime = new TimeOnly(10, 0),
                    EndTime = new TimeOnly(12, 0)
                },
                new Session
                {
                    Id = 4,
                    CourseId = 4,
                    InstructorId = 5,
                    ClassroomId = 4,
                    SessionDate = fixedDate.AddDays(3),
                    StartTime = new TimeOnly(13, 0),
                    EndTime = new TimeOnly(15, 0)
                },
                new Session
                {
                    Id = 5,
                    CourseId = 5,
                    InstructorId = 2,
                    ClassroomId = 5,
                    SessionDate = fixedDate.AddDays(4),
                    StartTime = new TimeOnly(14, 0),
                    EndTime = new TimeOnly(16, 0)
                }
            );

            // ENROLLMENT
            modelBuilder.Entity<Enrollment>().HasData(
                new Enrollment
                {
                    Id = 1,
                    TraineeId = 1,
                    SessionId = 1,
                    Status = EnrollmentStatus.ENROLLED,
                    EnrollmentDate = fixedDate
                },
                new Enrollment
                {
                    Id = 2,
                    TraineeId = 4,
                    SessionId = 1,
                    Status = EnrollmentStatus.CONFIRMED,
                    EnrollmentDate = fixedDate
                },
                new Enrollment
                {
                    Id = 3,
                    TraineeId = 1,
                    SessionId = 3,
                    Status = EnrollmentStatus.ATTENDING,
                    EnrollmentDate = fixedDate
                },
                new Enrollment
                {
                    Id = 4,
                    TraineeId = 4,
                    SessionId = 5,
                    Status = EnrollmentStatus.COMPLETED,
                    EnrollmentDate = fixedDate
                },
                new Enrollment
                {
                    Id = 5,
                    TraineeId = 1,
                    SessionId = 2,
                    Status = EnrollmentStatus.DROPPED,
                    EnrollmentDate = fixedDate
                }
            );

            // PAYMENT
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
                    Amount = 60,
                    PaymentDate = fixedDate,
                    Status = PaymentStatus.PARTIAL
                },
                new Payment
                {
                    Id = 3,
                    EnrollmentId = 3,
                    Amount = 150,
                    PaymentDate = fixedDate,
                    Status = PaymentStatus.FULL
                },
                new Payment
                {
                    Id = 4,
                    EnrollmentId = 4,
                    Amount = 50,
                    PaymentDate = fixedDate,
                    Status = PaymentStatus.PARTIAL
                },
                new Payment
                {
                    Id = 5,
                    EnrollmentId = 5,
                    Amount = 180,
                    PaymentDate = fixedDate,
                    Status = PaymentStatus.FULL
                }
            );

            // BALANCE
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
                    AmountDue = 60,
                    DueDate = fixedDate.AddDays(7)
                },
                new Balance
                {
                    Id = 3,
                    EnrollmentId = 3,
                    AmountDue = 0,
                    DueDate = fixedDate
                },
                new Balance
                {
                    Id = 4,
                    EnrollmentId = 4,
                    AmountDue = 50,
                    DueDate = fixedDate.AddDays(5)
                },
                new Balance
                {
                    Id = 5,
                    EnrollmentId = 5,
                    AmountDue = 0,
                    DueDate = fixedDate
                }
            );

            // ASSESSMENT
            modelBuilder.Entity<Assessment>().HasData(
                new Assessment
                {
                    Id = 1,
                    EnrollmentId = 1,
                    Status = AssessmentStatus.PENDING,
                    DueDate = fixedDate.AddDays(5)
                },
                new Assessment
                {
                    Id = 2,
                    EnrollmentId = 2,
                    Status = AssessmentStatus.PENDING,
                    DueDate = fixedDate.AddDays(5)
                },
                new Assessment
                {
                    Id = 3,
                    EnrollmentId = 3,
                    Status = AssessmentStatus.PASS,
                    DueDate = fixedDate.AddDays(5),
                    CompletedBy = fixedDate.AddDays(6)
                },
                new Assessment
                {
                    Id = 4,
                    EnrollmentId = 4,
                    Status = AssessmentStatus.PASS,
                    DueDate = fixedDate.AddDays(5),
                    CompletedBy = fixedDate.AddDays(6)
                },
                new Assessment
                {
                    Id = 5,
                    EnrollmentId = 5,
                    Status = AssessmentStatus.PENDING,
                    DueDate = fixedDate.AddDays(5)
                }
            );

            // NOTIFICATION
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
                    Message = "New session assigned",
                    CreatedDate = fixedDate,
                    Status = NotificationStatus.READ
                },
                new Notification
                {
                    Id = 3,
                    UserId = 3,
                    Message = "New enrollment received",
                    CreatedDate = fixedDate,
                    Status = NotificationStatus.UNREAD
                },
                new Notification
                {
                    Id = 4,
                    UserId = 4,
                    Message = "Payment reminder",
                    CreatedDate = fixedDate,
                    Status = NotificationStatus.UNREAD
                },
                new Notification
                {
                    Id = 5,
                    UserId = 5,
                    Message = "Schedule updated",
                    CreatedDate = fixedDate,
                    Status = NotificationStatus.READ
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
                },
                new InstructorAvailability
                {
                    Id = 2,
                    InstructorId = 5,
                    DayStart = Day.SUNDAY,
                    DayEnd = Day.THURSDAY,
                    StartTime = new TimeOnly(9, 0),
                    EndTime = new TimeOnly(17, 0)
                }
            );

            // TRAINEE CERTIFICATION
            modelBuilder.Entity<TraineeCertification>().HasData(
                new TraineeCertification
                {
                    Id = 1,
                    TraineeId = 1,
                    TrackId = 1,
                    // for public certification lookup
                    CertificateReferenceNumber = "CERT-1001",
                    Status = TraineeCertificationStatus.SUCCESS
                },
                new TraineeCertification
                {
                    Id = 2,
                    TraineeId = 4,
                    TrackId = 2,
                    CertificateReferenceNumber = "CERT-1002",
                    Status = TraineeCertificationStatus.SUCCESS
                },
                new TraineeCertification
                {
                    Id = 3,
                    TraineeId = 1,
                    TrackId = 3,
                    Status = TraineeCertificationStatus.FAILED
                },
                new TraineeCertification
                {
                    Id = 4,
                    TraineeId = 4,
                    TrackId = 4,
                    Status = TraineeCertificationStatus.FAILED
                },
                new TraineeCertification
                {
                    Id = 5,
                    TraineeId = 1,
                    TrackId = 5,
                    CertificateReferenceNumber = "CERT-1005",
                    Status = TraineeCertificationStatus.SUCCESS
                }
            );

            // COURSE TRACK
            modelBuilder.Entity("CourseTrack").HasData(
                new { CoursesId = 1, TracksId = 1 },
                new { CoursesId = 2, TracksId = 1 },
                new { CoursesId = 3, TracksId = 2 },
                new { CoursesId = 4, TracksId = 2 },
                new { CoursesId = 5, TracksId = 3 }
            );

            // CLASSROOM EQUIPMENT
            modelBuilder.Entity("ClassroomEquipment").HasData(
                new { ClassroomsId = 1, EquipmentsId = 1 },
                new { ClassroomsId = 2, EquipmentsId = 2 },
                new { ClassroomsId = 3, EquipmentsId = 3 },
                new { ClassroomsId = 4, EquipmentsId = 3 },
                new { ClassroomsId = 5, EquipmentsId = 4 }
            );

            // INSTRUCTOR EXPERTISE
            modelBuilder.Entity<InstructorExpertise>().HasData(
                new { InstructorId = 2, CourseId = 1 },
                new { InstructorId = 2, CourseId = 2 },
                new { InstructorId = 5, CourseId = 3 },
                new { InstructorId = 5, CourseId = 4 },
                new { InstructorId = 2, CourseId = 5 }
            );
        }
    }
} 