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

            var fixedDate = new DateTime(2026, 4, 1);
            // USERS
            modelBuilder.Entity<User>().HasData(
                new User { Id = 1, FirstName = "Ahmed", LastName = "AlMansouri", CPR = "860412345", Password = "$2a$12$Ys7YXxI9M7EqQY60T8aNFe31SwSs8IGXjfAYsFNp55NcGyzL4cIym", Role = UserRole.TRAINEE, Email = "ahmed@mail.com", Phone = "39001001", RegistrationDate = fixedDate },
                new User { Id = 2, FirstName = "Fatima", LastName = "Ali", CPR = "920815678", Password = "$2a$12$Ys7YXxI9M7EqQY60T8aNFe31SwSs8IGXjfAYsFNp55NcGyzL4cIym", Role = UserRole.TRAINEE, Email = "fatima@mail.com", Phone = "39001002", RegistrationDate = fixedDate },
                new User { Id = 3, FirstName = "Khalid", LastName = "AlDosari", CPR = "950322901", Password = "$2a$12$Ys7YXxI9M7EqQY60T8aNFe31SwSs8IGXjfAYsFNp55NcGyzL4cIym", Role = UserRole.TRAINEE, Email = "khalid@mail.com", Phone = "39001003", RegistrationDate = fixedDate.AddDays(5) },
                new User { Id = 4, FirstName = "Sara", LastName = "AlZayani", CPR = "780610234", Password = "$2a$12$Ys7YXxI9M7EqQY60T8aNFe31SwSs8IGXjfAYsFNp55NcGyzL4cIym", Role = UserRole.INSTRUCTOR, Email = "sara@mail.com", Phone = "39002001", RegistrationDate = fixedDate },
                new User { Id = 5, FirstName = "Hassan", LastName = "Ali", CPR = "820905567", Password = "$2a$12$Ys7YXxI9M7EqQY60T8aNFe31SwSs8IGXjfAYsFNp55NcGyzL4cIym", Role = UserRole.INSTRUCTOR, Email = "hassan@mail.com", Phone = "39002002", RegistrationDate = fixedDate },
                new User { Id = 6, FirstName = "Noor", LastName = "AlHammadi", CPR = "750318890", Password = "$2a$12$Ys7YXxI9M7EqQY60T8aNFe31SwSs8IGXjfAYsFNp55NcGyzL4cIym", Role = UserRole.INSTRUCTOR, Email = "noor@mail.com", Phone = "39002003", RegistrationDate = fixedDate },
                new User { Id = 7, FirstName = "Dana", LastName = "AlBanki", CPR = "810724112", Password = "$2a$12$Ys7YXxI9M7EqQY60T8aNFe31SwSs8IGXjfAYsFNp55NcGyzL4cIym", Role = UserRole.TRAINING_COORDINATOR, Email = "dana@mail.com", Phone = "39003001", RegistrationDate = fixedDate }
            );

            // COURSES
            modelBuilder.Entity<Course>().HasData(
                new Course { Id = 1, Category = CourseCategory.Programming, Title = "C# Fundamentals", Description = "Core concepts of C# programming including variables, control flow, and OOP.", Duration = 20, Capacity = 15, Fee = 120, PrerequisiteId = null },
                new Course { Id = 2, Category = CourseCategory.Programming, Title = "Advanced C# & .NET", Description = "Deep dive into LINQ, async/await, generics, and design patterns.", Duration = 30, Capacity = 12, Fee = 180, PrerequisiteId = 1 },
                new Course { Id = 3, Category = CourseCategory.WebDevelopment, Title = "ASP.NET Core MVC", Description = "Building web applications using ASP.NET Core MVC and Entity Framework.", Duration = 35, Capacity = 12, Fee = 200, PrerequisiteId = 2 },
                new Course { Id = 4, Category = CourseCategory.Database, Title = "SQL Server Fundamentals", Description = "Introduction to relational databases, T-SQL queries, and SQL Server.", Duration = 25, Capacity = 15, Fee = 150, PrerequisiteId = null },
                new Course { Id = 5, Category = CourseCategory.Database, Title = "Advanced SQL & Performance", Description = "Stored procedures, indexing, query optimization, and database design.", Duration = 30, Capacity = 12, Fee = 175, PrerequisiteId = 4 },
                new Course { Id = 6, Category = CourseCategory.Cybersecurity, Title = "Web Security Essentials", Description = "Common vulnerabilities, OWASP Top 10, secure coding practices.", Duration = 20, Capacity = 20, Fee = 140, PrerequisiteId = null }
            );

            // TRACKS
            modelBuilder.Entity<Track>().HasData(
                new Track { Id = 1, Name = "Full-Stack .NET Developer", Description = "Covers C#, Advanced C#, and ASP.NET Core MVC to build complete web applications." },
                new Track { Id = 2, Name = "Database Administrator", Description = "Covers SQL fundamentals and advanced SQL for database management and optimization." }
            );

            // CLASSROOMS
            modelBuilder.Entity<Classroom>().HasData(
                new Classroom { Id = 1, Name = "Room A101", Seats = 20 },
                new Classroom { Id = 2, Name = "Lab B202", Seats = 15 },
                new Classroom { Id = 3, Name = "Room C303", Seats = 25 }
            );

            // EQUIPMENT
            modelBuilder.Entity<Equipment>().HasData(
                new Equipment { Id = 1, Name = "Projector" },
                new Equipment { Id = 2, Name = "Lab Computers" },
                new Equipment { Id = 3, Name = "Whiteboard" }
            );

            // INSTRUCTOR AVAILABILITY
            modelBuilder.Entity<InstructorAvailability>().HasData(
                new InstructorAvailability { Id = 1, InstructorId = 4, DayStart = Day.SUNDAY, DayEnd = Day.THURSDAY, StartTime = new TimeOnly(8, 0), EndTime = new TimeOnly(16, 0) },
                new InstructorAvailability { Id = 2, InstructorId = 5, DayStart = Day.SUNDAY, DayEnd = Day.WEDNESDAY, StartTime = new TimeOnly(9, 0), EndTime = new TimeOnly(17, 0) },
                new InstructorAvailability { Id = 3, InstructorId = 6, DayStart = Day.MONDAY, DayEnd = Day.THURSDAY, StartTime = new TimeOnly(10, 0), EndTime = new TimeOnly(18, 0) }
            );

            // INSTRUCTOR EXPERTISE
            modelBuilder.Entity<InstructorExpertise>().HasData(
                new { InstructorId = 4, CourseId = 1 },
                new { InstructorId = 4, CourseId = 2 },
                new { InstructorId = 4, CourseId = 3 },
                new { InstructorId = 5, CourseId = 4 },
                new { InstructorId = 5, CourseId = 5 },
                new { InstructorId = 6, CourseId = 6 }
            );

            // SESSIONS
            modelBuilder.Entity<Session>().HasData(
                new Session { Id = 1, CourseId = 1, InstructorId = 4, ClassroomId = 1, SessionDate = new DateTime(2026, 4, 5), StartTime = new TimeOnly(9, 0), EndTime = new TimeOnly(12, 0) },
                new Session { Id = 2, CourseId = 2, InstructorId = 4, ClassroomId = 2, SessionDate = new DateTime(2026, 4, 20), StartTime = new TimeOnly(9, 0), EndTime = new TimeOnly(12, 0) },
                new Session { Id = 3, CourseId = 3, InstructorId = 4, ClassroomId = 2, SessionDate = new DateTime(2026, 5, 10), StartTime = new TimeOnly(9, 0), EndTime = new TimeOnly(12, 0) },
                new Session { Id = 4, CourseId = 4, InstructorId = 5, ClassroomId = 3, SessionDate = new DateTime(2026, 4, 8), StartTime = new TimeOnly(10, 0), EndTime = new TimeOnly(13, 0) },
                new Session { Id = 5, CourseId = 5, InstructorId = 5, ClassroomId = 3, SessionDate = new DateTime(2026, 4, 25), StartTime = new TimeOnly(10, 0), EndTime = new TimeOnly(13, 0) },
                new Session { Id = 6, CourseId = 6, InstructorId = 6, ClassroomId = 1, SessionDate = new DateTime(2026, 7, 1), StartTime = new TimeOnly(10, 0), EndTime = new TimeOnly(13, 0) }
            );

            // ENROLLMENTS
            modelBuilder.Entity<Enrollment>().HasData(
                new Enrollment { Id = 1, TraineeId = 1, SessionId = 1, Status = EnrollmentStatus.COMPLETED, EnrollmentDate = new DateTime(2026, 4, 1), CompletionDate = new DateTime(2026, 4, 5) },
                new Enrollment { Id = 2, TraineeId = 1, SessionId = 2, Status = EnrollmentStatus.COMPLETED, EnrollmentDate = new DateTime(2026, 4, 15), CompletionDate = new DateTime(2026, 4, 20) },
                new Enrollment { Id = 3, TraineeId = 1, SessionId = 3, Status = EnrollmentStatus.COMPLETED, EnrollmentDate = new DateTime(2026, 5, 1), CompletionDate = new DateTime(2026, 5, 10) },
                new Enrollment { Id = 4, TraineeId = 2, SessionId = 4, Status = EnrollmentStatus.COMPLETED, EnrollmentDate = new DateTime(2026, 4, 1), CompletionDate = new DateTime(2026, 4, 8) },
                new Enrollment { Id = 5, TraineeId = 2, SessionId = 5, Status = EnrollmentStatus.COMPLETED, EnrollmentDate = new DateTime(2026, 4, 20), CompletionDate = new DateTime(2026, 4, 25) },
                new Enrollment { Id = 6, TraineeId = 2, SessionId = 6, Status = EnrollmentStatus.ENROLLED, EnrollmentDate = new DateTime(2026, 5, 22) },
                new Enrollment { Id = 7, TraineeId = 3, SessionId = 1, Status = EnrollmentStatus.COMPLETED, EnrollmentDate = new DateTime(2026, 4, 1), CompletionDate = new DateTime(2026, 4, 5) },
                new Enrollment { Id = 8, TraineeId = 3, SessionId = 2, Status = EnrollmentStatus.ATTENDING, EnrollmentDate = new DateTime(2026, 4, 16) },
                new Enrollment { Id = 9, TraineeId = 3, SessionId = 4, Status = EnrollmentStatus.DROPPED, EnrollmentDate = new DateTime(2026, 4, 16) }
            );

            // PAYMENTS
            modelBuilder.Entity<Payment>().HasData(
                new Payment { Id = 1, EnrollmentId = 1, Amount = 120, PaymentDate = new DateTime(2026, 4, 1), Status = PaymentStatus.FULL },
                new Payment { Id = 2, EnrollmentId = 2, Amount = 180, PaymentDate = new DateTime(2026, 4, 15), Status = PaymentStatus.FULL },
                new Payment { Id = 3, EnrollmentId = 3, Amount = 200, PaymentDate = new DateTime(2026, 5, 1), Status = PaymentStatus.FULL },
                new Payment { Id = 4, EnrollmentId = 4, Amount = 150, PaymentDate = new DateTime(2026, 4, 1), Status = PaymentStatus.FULL },
                new Payment { Id = 5, EnrollmentId = 5, Amount = 175, PaymentDate = new DateTime(2026, 4, 20), Status = PaymentStatus.FULL },
                new Payment { Id = 6, EnrollmentId = 6, Amount = 70, PaymentDate = new DateTime(2026, 5, 22), Status = PaymentStatus.PARTIAL },
                new Payment { Id = 7, EnrollmentId = 7, Amount = 120, PaymentDate = new DateTime(2026, 4, 1), Status = PaymentStatus.FULL },
                new Payment { Id = 8, EnrollmentId = 8, Amount = 90, PaymentDate = new DateTime(2026, 4, 16), Status = PaymentStatus.PARTIAL },
                new Payment { Id = 9, EnrollmentId = 9, Amount = 60, PaymentDate = new DateTime(2026, 4, 16), Status = PaymentStatus.PARTIAL }
            );

            // BALANCES
            modelBuilder.Entity<Balance>().HasData(
                new Balance { Id = 1, EnrollmentId = 1, AmountDue = 0, DueDate = new DateTime(2026, 4, 1), Status = BalanceStatus.PAID },
                new Balance { Id = 2, EnrollmentId = 2, AmountDue = 0, DueDate = new DateTime(2026, 4, 15), Status = BalanceStatus.PAID },
                new Balance { Id = 3, EnrollmentId = 3, AmountDue = 0, DueDate = new DateTime(2026, 5, 1), Status = BalanceStatus.PAID },
                new Balance { Id = 4, EnrollmentId = 4, AmountDue = 0, DueDate = new DateTime(2026, 4, 1), Status = BalanceStatus.PAID },
                new Balance { Id = 5, EnrollmentId = 5, AmountDue = 0, DueDate = new DateTime(2026, 4, 20), Status = BalanceStatus.PAID },
                new Balance { Id = 6, EnrollmentId = 6, AmountDue = 70, DueDate = new DateTime(2026, 6, 5), Status = BalanceStatus.PENDING },
                new Balance { Id = 7, EnrollmentId = 7, AmountDue = 0, DueDate = new DateTime(2026, 4, 1), Status = BalanceStatus.PAID },
                new Balance { Id = 8, EnrollmentId = 8, AmountDue = 90, DueDate = new DateTime(2026, 5, 10), Status = BalanceStatus.OVERDUE },
                new Balance { Id = 9, EnrollmentId = 9, AmountDue = 90, DueDate = new DateTime(2026, 5, 1), Status = BalanceStatus.OVERDUE }
            );

            // ASSESSMENTS
            modelBuilder.Entity<Assessment>().HasData(
                new Assessment { Id = 1, EnrollmentId = 1, Status = AssessmentStatus.PASS, DueDate = new DateTime(2026, 4, 5), CompletedBy = new DateTime(2026, 4, 5) },
                new Assessment { Id = 2, EnrollmentId = 2, Status = AssessmentStatus.PASS, DueDate = new DateTime(2026, 4, 20), CompletedBy = new DateTime(2026, 4, 20) },
                new Assessment { Id = 3, EnrollmentId = 3, Status = AssessmentStatus.PASS, DueDate = new DateTime(2026, 5, 10), CompletedBy = new DateTime(2026, 5, 10) },
                new Assessment { Id = 4, EnrollmentId = 4, Status = AssessmentStatus.PASS, DueDate = new DateTime(2026, 4, 8), CompletedBy = new DateTime(2026, 4, 8) },
                new Assessment { Id = 5, EnrollmentId = 5, Status = AssessmentStatus.PASS, DueDate = new DateTime(2026, 4, 25), CompletedBy = new DateTime(2026, 4, 25) },
                new Assessment { Id = 6, EnrollmentId = 6, Status = AssessmentStatus.PENDING, DueDate = new DateTime(2026, 7, 10) },
                new Assessment { Id = 7, EnrollmentId = 7, Status = AssessmentStatus.PASS, DueDate = new DateTime(2026, 4, 5), CompletedBy = new DateTime(2026, 4, 5) },
                new Assessment { Id = 8, EnrollmentId = 8, Status = AssessmentStatus.PENDING, DueDate = new DateTime(2026, 5, 30) }
            );

            // FEEDBACKS
            modelBuilder.Entity<Feedback>().HasData(
                new Feedback { Id = 1, TraineeId = 1, InstructorId = 4, CourseId = 1, Rating = 5, Comment = "Sara explains concepts very clearly. Highly recommended.", SubmittedAt = new DateTime(2026, 4, 21), ContentRating = 5, InstructorRating = 5, OrganizationRating = 4, RecommendCourse = true },
                new Feedback { Id = 2, TraineeId = 2, InstructorId = 5, CourseId = 4, Rating = 5, Comment = "Hassan is a great instructor. The hands-on labs were very helpful.", SubmittedAt = new DateTime(2026, 4, 23), ContentRating = 4, InstructorRating = 5, OrganizationRating = 5, RecommendCourse = true },
                new Feedback { Id = 3, TraineeId = 3, InstructorId = 4, CourseId = 1, Rating = 4, Comment = "Good introduction. Would have liked more exercises.", SubmittedAt = new DateTime(2026, 4, 21), ContentRating = 4, InstructorRating = 4, OrganizationRating = 3, RecommendCourse = true }
            );

            // NOTIFICATIONS
            modelBuilder.Entity<Notification>().HasData(
                new Notification { Id = 1, UserId = 1, Message = "Welcome to the Training Platform, Ahmed!", CreatedDate = fixedDate, Status = NotificationStatus.UNREAD },
                new Notification { Id = 2, UserId = 2, Message = "Welcome to the Training Platform, Fatima!", CreatedDate = fixedDate, Status = NotificationStatus.UNREAD },
                new Notification { Id = 3, UserId = 3, Message = "Welcome to the Training Platform, Khalid!", CreatedDate = fixedDate.AddDays(5), Status = NotificationStatus.UNREAD },
                new Notification { Id = 4, UserId = 1, Message = "Your enrollment in C# Fundamentals has been confirmed.", CreatedDate = new DateTime(2026, 4, 15), Status = NotificationStatus.READ },
                new Notification { Id = 5, UserId = 1, Message = "Congratulations! You have completed the Full-Stack .NET Developer track.", CreatedDate = new DateTime(2026, 5, 11), Status = NotificationStatus.UNREAD },
                new Notification { Id = 6, UserId = 2, Message = "Your enrollment in SQL Server Fundamentals has been confirmed.", CreatedDate = new DateTime(2026, 4, 15), Status = NotificationStatus.READ },
                new Notification { Id = 7, UserId = 2, Message = "Reminder: Your balance for Web Security is pending.", CreatedDate = new DateTime(2026, 5, 22), Status = NotificationStatus.UNREAD },
                new Notification { Id = 8, UserId = 3, Message = "Reminder: Your balance for Advanced C# is overdue.", CreatedDate = new DateTime(2026, 5, 20), Status = NotificationStatus.UNREAD },
                new Notification { Id = 9, UserId = 4, Message = "You have 3 sessions scheduled this month.", CreatedDate = fixedDate, Status = NotificationStatus.UNREAD },
                new Notification { Id = 10, UserId = 5, Message = "You have 2 sessions scheduled this month.", CreatedDate = fixedDate, Status = NotificationStatus.UNREAD }
            );

            // TRAINEE CERTIFICATIONS
            modelBuilder.Entity<TraineeCertification>().HasData(
                new TraineeCertification { Id = 1, TraineeId = 1, TrackId = 1, CertificateReferenceNumber = "CERT-NET-2026-001", Status = TraineeCertificationStatus.SUCCESS },
                new TraineeCertification { Id = 2, TraineeId = 2, TrackId = 2, CertificateReferenceNumber = "CERT-DBA-2026-001", Status = TraineeCertificationStatus.SUCCESS }
            );

            // COURSE TRACK
            modelBuilder.Entity("CourseTrack").HasData(
                new { CoursesId = 1, TracksId = 1 },
                new { CoursesId = 2, TracksId = 1 },
                new { CoursesId = 3, TracksId = 1 },
                new { CoursesId = 4, TracksId = 2 },
                new { CoursesId = 5, TracksId = 2 }
            );

            // CLASSROOM EQUIPMENT
            modelBuilder.Entity("ClassroomEquipment").HasData(
                new { ClassroomsId = 1, EquipmentsId = 1 },
                new { ClassroomsId = 1, EquipmentsId = 3 },
                new { ClassroomsId = 2, EquipmentsId = 2 },
                new { ClassroomsId = 2, EquipmentsId = 1 },
                new { ClassroomsId = 3, EquipmentsId = 1 },
                new { ClassroomsId = 3, EquipmentsId = 3 }
            );

        }
    }
}