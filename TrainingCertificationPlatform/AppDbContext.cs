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

<<<<<<< HEAD
            // SEED DATA
=======
            // SeedData (Data to test)
>>>>>>> fd2e9fb (making it up-to-date)

            var fixedDate = new DateTime(2026, 4, 4);

            // USERS

            modelBuilder.Entity<User>().HasData(
<<<<<<< HEAD
=======
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
>>>>>>> fd2e9fb (making it up-to-date)

                // TRAINEES
                new User
                {
                    Id = 1,
                    FirstName = "Ali",
                    LastName = "Ahmad",
                    CPR = "123456789",
                    Password = "$2a$12$ZPzIhfjkDv3uc/4fEkhAfuAM/hYixvISLMEhyBYk7dxrsGJdw15Rq",
                    Role = UserRole.TRAINEE,
                    Email = "ali@mail.com",
                    Phone = "99999991",
                    RegistrationDate = fixedDate
                },

                new User
                {
                    Id = 4,
                    FirstName = "Omar",
                    LastName = "Ali",
                    CPR = "098765432",
                    Password = "$2a$12$ZPzIhfjkDv3uc/4fEkhAfuAM/hYixvISLMEhyBYk7dxrsGJdw15Rq",
                    Role = UserRole.TRAINEE,
                    Email = "omar@mail.com",
                    Phone = "99999994",
                    RegistrationDate = fixedDate
                },

                new User
                {
                    Id = 6,
                    FirstName = "Mariam",
                    LastName = "Hasan",
                    CPR = "444555666",
                    Password = "$2a$12$ZPzIhfjkDv3uc/4fEkhAfuAM/hYixvISLMEhyBYk7dxrsGJdw15Rq",
                    Role = UserRole.TRAINEE,
                    Email = "mariam@mail.com",
                    Phone = "99999996",
                    RegistrationDate = fixedDate
                },

                // INSTRUCTORS
                new User
                {
                    Id = 2,
                    FirstName = "Sara",
                    LastName = "Mohamed",
                    CPR = "987654321",
                    Password = "$2a$12$ZPzIhfjkDv3uc/4fEkhAfuAM/hYixvISLMEhyBYk7dxrsGJdw15Rq",
                    Role = UserRole.INSTRUCTOR,
                    Email = "sara@mail.com",
                    Phone = "99999992",
                    RegistrationDate = fixedDate
                },

                new User
                {
                    Id = 5,
                    FirstName = "Fatima",
                    LastName = "Yousef",
                    CPR = "012345678",
                    Password = "$2a$12$ZPzIhfjkDv3uc/4fEkhAfuAM/hYixvISLMEhyBYk7dxrsGJdw15Rq",
                    Role = UserRole.INSTRUCTOR,
                    Email = "fatima@mail.com",
                    Phone = "99999995",
                    RegistrationDate = fixedDate
                },

                // COORDINATOR
                new User
                {
                    Id = 3,
                    FirstName = "Dana",
                    LastName = "Albanki",
                    CPR = "112233445",
                    Password = "$2a$12$ZPzIhfjkDv3uc/4fEkhAfuAM/hYixvISLMEhyBYk7dxrsGJdw15Rq",
                    Role = UserRole.TRAINING_COORDINATOR,
                    Email = "dana@mail.com",
                    Phone = "99999993",
                    RegistrationDate = fixedDate
                }
            );

            // COURSES

            modelBuilder.Entity<Course>().HasData(
<<<<<<< HEAD

=======
>>>>>>> fd2e9fb (making it up-to-date)
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
<<<<<<< HEAD

=======
>>>>>>> fd2e9fb (making it up-to-date)
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
<<<<<<< HEAD

=======
>>>>>>> fd2e9fb (making it up-to-date)
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
<<<<<<< HEAD

=======
>>>>>>> fd2e9fb (making it up-to-date)
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
<<<<<<< HEAD

=======
>>>>>>> fd2e9fb (making it up-to-date)
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

            // TRACKS

            modelBuilder.Entity<Track>().HasData(
<<<<<<< HEAD

=======
>>>>>>> fd2e9fb (making it up-to-date)
                new Track
                {
                    Id = 1,
                    Name = "Backend Development",
                    Description = "Backend programming track"
                },
<<<<<<< HEAD

=======
>>>>>>> fd2e9fb (making it up-to-date)
                new Track
                {
                    Id = 2,
                    Name = "Database Administration",
                    Description = "Database management track"
                },
<<<<<<< HEAD

=======
>>>>>>> fd2e9fb (making it up-to-date)
                new Track
                {
                    Id = 3,
                    Name = "Networking Essentials",
                    Description = "Networking certification track"
<<<<<<< HEAD
=======
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
>>>>>>> fd2e9fb (making it up-to-date)
                }
            );

            // CLASSROOMS

            modelBuilder.Entity<Classroom>().HasData(
<<<<<<< HEAD

=======
>>>>>>> fd2e9fb (making it up-to-date)
                new Classroom { Id = 1, Name = "Room A", Seats = 30 },
                new Classroom { Id = 2, Name = "Room B", Seats = 25 },
                new Classroom { Id = 3, Name = "Lab 1", Seats = 20 },
                new Classroom { Id = 4, Name = "Lab 2", Seats = 20 },
                new Classroom { Id = 5, Name = "Conference Hall", Seats = 50 }
            );

            // EQUIPMENT

            modelBuilder.Entity<Equipment>().HasData(
<<<<<<< HEAD

                new Equipment { Id = 1, Name = "Projector" },
                new Equipment { Id = 2, Name = "Whiteboard" },
                new Equipment { Id = 3, Name = "Lab Computers" },
                new Equipment { Id = 4, Name = "Networking Kit" }
=======
                new Equipment { Id = 1, Name = "Projector" },
                new Equipment { Id = 2, Name = "Whiteboard" },
                new Equipment { Id = 3, Name = "Lab Computers" },
                new Equipment { Id = 4, Name = "Microphones" },
                new Equipment { Id = 5, Name = "Networking Kit" }
>>>>>>> fd2e9fb (making it up-to-date)
            );

            // SESSIONS

            modelBuilder.Entity<Session>().HasData(
<<<<<<< HEAD

                // UPCOMING
=======
>>>>>>> fd2e9fb (making it up-to-date)
                new Session
                {
                    Id = 1,
                    CourseId = 1,
                    InstructorId = 2,
                    ClassroomId = 1,
<<<<<<< HEAD
                    SessionDate = fixedDate.AddDays(1),
                    StartTime = new TimeOnly(9, 0),
                    EndTime = new TimeOnly(11, 0)
                },

=======
                    SessionDate = fixedDate,
                    StartTime = new TimeOnly(9, 0),
                    EndTime = new TimeOnly(11, 0)
                },
>>>>>>> fd2e9fb (making it up-to-date)
                new Session
                {
                    Id = 2,
                    CourseId = 2,
                    InstructorId = 2,
                    ClassroomId = 2,
<<<<<<< HEAD
                    SessionDate = fixedDate.AddDays(2),
                    StartTime = new TimeOnly(12, 0),
                    EndTime = new TimeOnly(14, 0)
                },

=======
                    SessionDate = fixedDate.AddDays(1),
                    StartTime = new TimeOnly(12, 0),
                    EndTime = new TimeOnly(14, 0)
                },
>>>>>>> fd2e9fb (making it up-to-date)
                new Session
                {
                    Id = 3,
                    CourseId = 3,
                    InstructorId = 5,
                    ClassroomId = 3,
<<<<<<< HEAD
                    SessionDate = fixedDate.AddDays(3),
                    StartTime = new TimeOnly(10, 0),
                    EndTime = new TimeOnly(12, 0)
                },

                // PAST
=======
                    SessionDate = fixedDate.AddDays(2),
                    StartTime = new TimeOnly(10, 0),
                    EndTime = new TimeOnly(12, 0)
                },
>>>>>>> fd2e9fb (making it up-to-date)
                new Session
                {
                    Id = 4,
                    CourseId = 4,
                    InstructorId = 5,
                    ClassroomId = 4,
<<<<<<< HEAD
                    SessionDate = fixedDate.AddDays(-5),
                    StartTime = new TimeOnly(13, 0),
                    EndTime = new TimeOnly(15, 0)
                },

=======
                    SessionDate = fixedDate.AddDays(3),
                    StartTime = new TimeOnly(13, 0),
                    EndTime = new TimeOnly(15, 0)
                },
>>>>>>> fd2e9fb (making it up-to-date)
                new Session
                {
                    Id = 5,
                    CourseId = 5,
                    InstructorId = 2,
                    ClassroomId = 5,
<<<<<<< HEAD
                    SessionDate = fixedDate.AddDays(-3),
=======
                    SessionDate = fixedDate.AddDays(4),
>>>>>>> fd2e9fb (making it up-to-date)
                    StartTime = new TimeOnly(14, 0),
                    EndTime = new TimeOnly(16, 0)
                }
            );

            // ENROLLMENTS

            modelBuilder.Entity<Enrollment>().HasData(
<<<<<<< HEAD

                // ENROLLED (JUST REGISTERED)
=======
>>>>>>> fd2e9fb (making it up-to-date)
                new Enrollment
                {
                    Id = 1,
                    TraineeId = 1,
                    SessionId = 1,
                    Status = EnrollmentStatus.ENROLLED,
                    EnrollmentDate = fixedDate
                },
<<<<<<< HEAD

                // CONFIRMED (PAYMENT CONFIRMED)
=======
>>>>>>> fd2e9fb (making it up-to-date)
                new Enrollment
                {
                    Id = 2,
                    TraineeId = 4,
                    SessionId = 1,
                    Status = EnrollmentStatus.CONFIRMED,
                    EnrollmentDate = fixedDate
                },
<<<<<<< HEAD

                // ATTENDING (CURRENTLY ATTENDING)
                new Enrollment
                {
                    Id = 3,
                    TraineeId = 6,
=======
                new Enrollment
                {
                    Id = 3,
                    TraineeId = 1,
>>>>>>> fd2e9fb (making it up-to-date)
                    SessionId = 3,
                    Status = EnrollmentStatus.ATTENDING,
                    EnrollmentDate = fixedDate
                },
<<<<<<< HEAD

                // COMPLETED AND PASSED
                new Enrollment
                {
                    Id = 4,
                    TraineeId = 1,
                    SessionId = 4,
                    Status = EnrollmentStatus.COMPLETED,
                    EnrollmentDate = fixedDate.AddDays(-10),
                    CompletionDate = fixedDate.AddDays(-2)
                },

                // DROPPED 
                new Enrollment
                {
                    Id = 5,
                    TraineeId = 4,
=======
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
>>>>>>> fd2e9fb (making it up-to-date)
                    SessionId = 2,
                    Status = EnrollmentStatus.DROPPED,
                    EnrollmentDate = fixedDate
                },
<<<<<<< HEAD

                // COMPLETED BUT FAILED
                new Enrollment
                {
                    Id = 6,
                    TraineeId = 6,
                    SessionId = 5,
                    Status = EnrollmentStatus.COMPLETED,
                    EnrollmentDate = fixedDate.AddDays(-8),
                    CompletionDate = fixedDate.AddDays(-1)
=======
                new Enrollment
                {
                    Id = 6,
                    TraineeId = 1,
                    SessionId = 2,
                    Status = EnrollmentStatus.COMPLETED,
                    EnrollmentDate = fixedDate
>>>>>>> fd2e9fb (making it up-to-date)
                }
            );

            // PAYMENTS

            modelBuilder.Entity<Payment>().HasData(
<<<<<<< HEAD

=======
>>>>>>> fd2e9fb (making it up-to-date)
                new Payment
                {
                    Id = 1,
                    EnrollmentId = 1,
                    Amount = 120,
                    PaymentDate = fixedDate,
                    Status = PaymentStatus.FULL
                },
<<<<<<< HEAD

=======
>>>>>>> fd2e9fb (making it up-to-date)
                new Payment
                {
                    Id = 2,
                    EnrollmentId = 2,
                    Amount = 60,
                    PaymentDate = fixedDate,
                    Status = PaymentStatus.PARTIAL
                },
<<<<<<< HEAD

=======
>>>>>>> fd2e9fb (making it up-to-date)
                new Payment
                {
                    Id = 3,
                    EnrollmentId = 3,
                    Amount = 150,
                    PaymentDate = fixedDate,
                    Status = PaymentStatus.FULL
                },
<<<<<<< HEAD

=======
>>>>>>> fd2e9fb (making it up-to-date)
                new Payment
                {
                    Id = 4,
                    EnrollmentId = 4,
<<<<<<< HEAD
                    Amount = 180,
                    PaymentDate = fixedDate,
                    Status = PaymentStatus.FULL
                },

=======
                    Amount = 50,
                    PaymentDate = fixedDate,
                    Status = PaymentStatus.PARTIAL
                },
>>>>>>> fd2e9fb (making it up-to-date)
                new Payment
                {
                    Id = 5,
                    EnrollmentId = 5,
<<<<<<< HEAD
                    Amount = 170,
=======
                    Amount = 180,
>>>>>>> fd2e9fb (making it up-to-date)
                    PaymentDate = fixedDate,
                    Status = PaymentStatus.FULL
                }
            );

            // BALANCES

            modelBuilder.Entity<Balance>().HasData(
<<<<<<< HEAD

=======
>>>>>>> fd2e9fb (making it up-to-date)
                new Balance
                {
                    Id = 1,
                    EnrollmentId = 1,
                    AmountDue = 0,
                    DueDate = fixedDate
                },
<<<<<<< HEAD

=======
>>>>>>> fd2e9fb (making it up-to-date)
                new Balance
                {
                    Id = 2,
                    EnrollmentId = 2,
                    AmountDue = 60,
                    DueDate = fixedDate.AddDays(7)
                },
<<<<<<< HEAD

=======
>>>>>>> fd2e9fb (making it up-to-date)
                new Balance
                {
                    Id = 3,
                    EnrollmentId = 3,
                    AmountDue = 0,
                    DueDate = fixedDate
                },
<<<<<<< HEAD

=======
>>>>>>> fd2e9fb (making it up-to-date)
                new Balance
                {
                    Id = 4,
                    EnrollmentId = 4,
<<<<<<< HEAD
=======
                    AmountDue = 50,
                    DueDate = fixedDate.AddDays(5)
                },
                new Balance
                {
                    Id = 5,
                    EnrollmentId = 5,
>>>>>>> fd2e9fb (making it up-to-date)
                    AmountDue = 0,
                    DueDate = fixedDate
                }
            );

            // ASSESSMENTS

            modelBuilder.Entity<Assessment>().HasData(
<<<<<<< HEAD

                // ENROLLED (PENDING)
=======
>>>>>>> fd2e9fb (making it up-to-date)
                new Assessment
                {
                    Id = 1,
                    EnrollmentId = 1,
<<<<<<< HEAD
                    Status = AssessmentStatus.PENDING,
                    DueDate = fixedDate.AddDays(5)
                },

                // CONFIRMED
=======
                    Status = AssessmentStatus.PASS,
                    DueDate = fixedDate.AddDays(5),
                    CompletedBy = fixedDate.AddDays(6)
                },
>>>>>>> fd2e9fb (making it up-to-date)
                new Assessment
                {
                    Id = 2,
                    EnrollmentId = 2,
                    Status = AssessmentStatus.PENDING,
                    DueDate = fixedDate.AddDays(5)
                },
<<<<<<< HEAD

                // ATTENDING
=======
>>>>>>> fd2e9fb (making it up-to-date)
                new Assessment
                {
                    Id = 3,
                    EnrollmentId = 3,
<<<<<<< HEAD
                    Status = AssessmentStatus.PENDING,
                    DueDate = fixedDate.AddDays(3)
                },

                // COMPLETED (PASS)
=======
                    Status = AssessmentStatus.PASS,
                    DueDate = fixedDate.AddDays(5),
                    CompletedBy = fixedDate.AddDays(6)
                },
>>>>>>> fd2e9fb (making it up-to-date)
                new Assessment
                {
                    Id = 4,
                    EnrollmentId = 4,
                    Status = AssessmentStatus.PASS,
<<<<<<< HEAD
                    DueDate = fixedDate.AddDays(-3),
                    CompletedBy = fixedDate.AddDays(-2)
                },

                // COMPLETED (FAIL)
                new Assessment
                {
                    Id = 5,
                    EnrollmentId = 6,
                    Status = AssessmentStatus.FAIL,
                    DueDate = fixedDate.AddDays(-2),
                    CompletedBy = fixedDate.AddDays(-1)
=======
                    DueDate = fixedDate.AddDays(5),
                    CompletedBy = fixedDate.AddDays(6)
                },
                new Assessment
                {
                    Id = 5,
                    EnrollmentId = 5,
                    Status = AssessmentStatus.PENDING,
                    DueDate = fixedDate.AddDays(5)
                },
                new Assessment
                {
                    Id = 6,
                    EnrollmentId = 6,
                    Status = AssessmentStatus.PASS,
                    DueDate = fixedDate.AddDays(5),
                    CompletedBy = fixedDate.AddDays(6)
>>>>>>> fd2e9fb (making it up-to-date)
                }
            );


            // NOTIFICATIONS

            modelBuilder.Entity<Notification>().HasData(
<<<<<<< HEAD

=======
>>>>>>> fd2e9fb (making it up-to-date)
                new Notification
                {
                    Id = 1,
                    UserId = 1,
                    Message = "Welcome to the platform",
                    CreatedDate = fixedDate,
                    Status = NotificationStatus.UNREAD
                },
<<<<<<< HEAD

=======
>>>>>>> fd2e9fb (making it up-to-date)
                new Notification
                {
                    Id = 2,
                    UserId = 2,
                    Message = "New session assigned",
                    CreatedDate = fixedDate,
                    Status = NotificationStatus.READ
                },
<<<<<<< HEAD

                new Notification
                {
                    Id = 3,
=======
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
>>>>>>> fd2e9fb (making it up-to-date)
                    UserId = 4,
                    Message = "Payment reminder",
                    CreatedDate = fixedDate,
                    Status = NotificationStatus.UNREAD
<<<<<<< HEAD
=======
                },
                new Notification
                {
                    Id = 5,
                    UserId = 5,
                    Message = "Schedule updated",
                    CreatedDate = fixedDate,
                    Status = NotificationStatus.READ
>>>>>>> fd2e9fb (making it up-to-date)
                }
            );

            // INSTRUCTOR AVAILABILITY

            modelBuilder.Entity<InstructorAvailability>().HasData(
<<<<<<< HEAD

=======
>>>>>>> fd2e9fb (making it up-to-date)
                new InstructorAvailability
                {
                    Id = 1,
                    InstructorId = 2,
                    DayStart = Day.SUNDAY,
                    DayEnd = Day.THURSDAY,
                    StartTime = new TimeOnly(8, 0),
                    EndTime = new TimeOnly(16, 0)
                },
<<<<<<< HEAD

=======
>>>>>>> fd2e9fb (making it up-to-date)
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

            // TRAINEE CERTIFICATIONS

            modelBuilder.Entity<TraineeCertification>().HasData(
<<<<<<< HEAD

=======
>>>>>>> fd2e9fb (making it up-to-date)
                new TraineeCertification
                {
                    Id = 1,
                    TraineeId = 1,
                    TrackId = 1,
<<<<<<< HEAD
                    CertificateReferenceNumber = "CERT-1001",
                    Status = TraineeCertificationStatus.SUCCESS
                },

=======
                    // for public certification lookup
                    CertificateReferenceNumber = "CERT-1001",
                    Status = TraineeCertificationStatus.SUCCESS
                },
>>>>>>> fd2e9fb (making it up-to-date)
                new TraineeCertification
                {
                    Id = 2,
                    TraineeId = 4,
                    TrackId = 2,
                    CertificateReferenceNumber = "CERT-1002",
                    Status = TraineeCertificationStatus.SUCCESS
                },
<<<<<<< HEAD

                new TraineeCertification
                {
                    Id = 3,
                    TraineeId = 6,
                    TrackId = 3,
                    Status = TraineeCertificationStatus.FAILED
=======
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
>>>>>>> fd2e9fb (making it up-to-date)
                }
            );

            // COURSE TRACK
<<<<<<< HEAD

=======
>>>>>>> fd2e9fb (making it up-to-date)
            modelBuilder.Entity("CourseTrack").HasData(

                new { CoursesId = 1, TracksId = 1 },
                new { CoursesId = 2, TracksId = 1 },
<<<<<<< HEAD

                new { CoursesId = 3, TracksId = 2 },
                new { CoursesId = 4, TracksId = 2 },

=======
                new { CoursesId = 3, TracksId = 2 },
                new { CoursesId = 4, TracksId = 2 },
>>>>>>> fd2e9fb (making it up-to-date)
                new { CoursesId = 5, TracksId = 3 }
            );

            // CLASSROOM EQUIPMENT
<<<<<<< HEAD

            modelBuilder.Entity("ClassroomEquipment").HasData(

                new { ClassroomsId = 1, EquipmentsId = 1 },
                new { ClassroomsId = 1, EquipmentsId = 2 },

                new { ClassroomsId = 2, EquipmentsId = 1 },

                new { ClassroomsId = 3, EquipmentsId = 3 },

                new { ClassroomsId = 4, EquipmentsId = 3 },

=======
            modelBuilder.Entity("ClassroomEquipment").HasData(
                new { ClassroomsId = 1, EquipmentsId = 1 },
                new { ClassroomsId = 2, EquipmentsId = 2 },
                new { ClassroomsId = 3, EquipmentsId = 3 },
                new { ClassroomsId = 4, EquipmentsId = 3 },
>>>>>>> fd2e9fb (making it up-to-date)
                new { ClassroomsId = 5, EquipmentsId = 4 }
            );

            // INSTRUCTOR EXPERTISE
<<<<<<< HEAD

=======
>>>>>>> fd2e9fb (making it up-to-date)
            modelBuilder.Entity<InstructorExpertise>().HasData(

                new { InstructorId = 2, CourseId = 1 },
                new { InstructorId = 2, CourseId = 2 },
<<<<<<< HEAD
                new { InstructorId = 2, CourseId = 5 },

                new { InstructorId = 5, CourseId = 3 },
                new { InstructorId = 5, CourseId = 4 }
=======
                new { InstructorId = 5, CourseId = 3 },
                new { InstructorId = 5, CourseId = 4 },
                new { InstructorId = 2, CourseId = 5 }
>>>>>>> fd2e9fb (making it up-to-date)
            );
        }
    }
} 