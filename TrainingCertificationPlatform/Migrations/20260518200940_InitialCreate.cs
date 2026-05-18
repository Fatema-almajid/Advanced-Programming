using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace TrainingCertificationPlatform.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Classrooms",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Seats = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Classrooms", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Courses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Category = table.Column<int>(type: "int", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PrerequisiteId = table.Column<int>(type: "int", nullable: true),
                    Duration = table.Column<int>(type: "int", nullable: false),
                    Capacity = table.Column<int>(type: "int", nullable: false),
                    Fee = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Courses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Courses_Courses_PrerequisiteId",
                        column: x => x.PrerequisiteId,
                        principalTable: "Courses",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Equipments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Equipments", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Tracks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tracks", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FirstName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Role = table.Column<int>(type: "int", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Phone = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RegistrationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CPR = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ClassroomEquipment",
                columns: table => new
                {
                    ClassroomsId = table.Column<int>(type: "int", nullable: false),
                    EquipmentsId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClassroomEquipment", x => new { x.ClassroomsId, x.EquipmentsId });
                    table.ForeignKey(
                        name: "FK_ClassroomEquipment_Classrooms_ClassroomsId",
                        column: x => x.ClassroomsId,
                        principalTable: "Classrooms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ClassroomEquipment_Equipments_EquipmentsId",
                        column: x => x.EquipmentsId,
                        principalTable: "Equipments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CourseTrack",
                columns: table => new
                {
                    CoursesId = table.Column<int>(type: "int", nullable: false),
                    TracksId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CourseTrack", x => new { x.CoursesId, x.TracksId });
                    table.ForeignKey(
                        name: "FK_CourseTrack_Courses_CoursesId",
                        column: x => x.CoursesId,
                        principalTable: "Courses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CourseTrack_Tracks_TracksId",
                        column: x => x.TracksId,
                        principalTable: "Tracks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "InstructorAvailabilities",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    InstructorId = table.Column<int>(type: "int", nullable: false),
                    DayStart = table.Column<int>(type: "int", nullable: false),
                    DayEnd = table.Column<int>(type: "int", nullable: false),
                    StartTime = table.Column<TimeOnly>(type: "time", nullable: false),
                    EndTime = table.Column<TimeOnly>(type: "time", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InstructorAvailabilities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InstructorAvailabilities_Users_InstructorId",
                        column: x => x.InstructorId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "InstructorExpertises",
                columns: table => new
                {
                    InstructorId = table.Column<int>(type: "int", nullable: false),
                    CourseId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InstructorExpertises", x => new { x.InstructorId, x.CourseId });
                    table.ForeignKey(
                        name: "FK_InstructorExpertises_Courses_CourseId",
                        column: x => x.CourseId,
                        principalTable: "Courses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_InstructorExpertises_Users_InstructorId",
                        column: x => x.InstructorId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Notifications",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    Message = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notifications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Notifications_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Sessions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CourseId = table.Column<int>(type: "int", nullable: false),
                    InstructorId = table.Column<int>(type: "int", nullable: false),
                    ClassroomId = table.Column<int>(type: "int", nullable: false),
                    SessionDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    StartTime = table.Column<TimeOnly>(type: "time", nullable: false),
                    EndTime = table.Column<TimeOnly>(type: "time", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sessions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Sessions_Classrooms_ClassroomId",
                        column: x => x.ClassroomId,
                        principalTable: "Classrooms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Sessions_Courses_CourseId",
                        column: x => x.CourseId,
                        principalTable: "Courses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Sessions_Users_InstructorId",
                        column: x => x.InstructorId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TraineeCertifications",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TraineeId = table.Column<int>(type: "int", nullable: false),
                    TrackId = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    CertificateReferenceNumber = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TraineeCertifications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TraineeCertifications_Tracks_TrackId",
                        column: x => x.TrackId,
                        principalTable: "Tracks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TraineeCertifications_Users_TraineeId",
                        column: x => x.TraineeId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Enrollments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TraineeId = table.Column<int>(type: "int", nullable: false),
                    SessionId = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    EnrollmentDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CompletionDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    PaymentDueDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UserId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Enrollments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Enrollments_Sessions_SessionId",
                        column: x => x.SessionId,
                        principalTable: "Sessions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Enrollments_Users_TraineeId",
                        column: x => x.TraineeId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Enrollments_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Assessments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EnrollmentId = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    DueDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CompletedBy = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Assessments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Assessments_Enrollments_EnrollmentId",
                        column: x => x.EnrollmentId,
                        principalTable: "Enrollments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Balances",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EnrollmentId = table.Column<int>(type: "int", nullable: false),
                    AmountDue = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: false),
                    DueDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Balances", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Balances_Enrollments_EnrollmentId",
                        column: x => x.EnrollmentId,
                        principalTable: "Enrollments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Payments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EnrollmentId = table.Column<int>(type: "int", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: false),
                    PaymentDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Payments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Payments_Enrollments_EnrollmentId",
                        column: x => x.EnrollmentId,
                        principalTable: "Enrollments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Classrooms",
                columns: new[] { "Id", "Name", "Seats" },
                values: new object[,]
                {
                    { 1, "Room A", 30 },
                    { 2, "Room B", 25 },
                    { 3, "Lab 1", 20 },
                    { 4, "Lab 2", 20 },
                    { 5, "Conference Hall", 50 }
                });

            migrationBuilder.InsertData(
                table: "Courses",
                columns: new[] { "Id", "Capacity", "Category", "Description", "Duration", "Fee", "PrerequisiteId", "Title" },
                values: new object[,]
                {
                    { 1, 25, 1, "Introduction to C# programming", 20, 120.0, null, "C# Basics" },
                    { 3, 30, 2, "Introduction to SQL Server", 25, 150.0, null, "SQL Fundamentals" },
                    { 5, 25, 4, "Introduction to networking", 15, 100.0, null, "Networking Basics" }
                });

            migrationBuilder.InsertData(
                table: "Equipments",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "Projector" },
                    { 2, "Whiteboard" },
                    { 3, "Lab Computers" },
                    { 4, "Microphones" },
                    { 5, "Networking Kit" }
                });

            migrationBuilder.InsertData(
                table: "Tracks",
                columns: new[] { "Id", "Description", "Name" },
                values: new object[,]
                {
                    { 1, "Backend programming track", "Backend Development" },
                    { 2, "Database management track", "Database Administration" },
                    { 3, "Networking certification track", "Networking Essentials" },
                    { 4, "Complete web development track", "Full Stack Development" },
                    { 5, "Software engineering foundations", "Software Engineering" }
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "CPR", "Email", "FirstName", "LastName", "Password", "Phone", "RegistrationDate", "Role" },
                values: new object[,]
                {
                    { 1, "123456789", "ali@mail.com", "Ali", "Ahmad", "$2a$12$ZPzIhfjkDv3uc/4fEkhAfuAM/hYixvISLMEhyBYk7dxrsGJdw15Rq", "99999991", new DateTime(2026, 4, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), 0 },
                    { 2, "987654321", "sara@mail.com", "Sara", "Mohamed", "$2a$12$ZPzIhfjkDv3uc/4fEkhAfuAM/hYixvISLMEhyBYk7dxrsGJdw15Rq", "99999992", new DateTime(2026, 4, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), 1 },
                    { 3, "112233445", "dana@mail.com", "Dana", "Albanki", "$2a$12$ZPzIhfjkDv3uc/4fEkhAfuAM/hYixvISLMEhyBYk7dxrsGJdw15Rq", "99999993", new DateTime(2026, 4, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), 2 },
                    { 4, "098765432", "omar@mail.com", "Omar", "Ali", "$2a$12$ZPzIhfjkDv3uc/4fEkhAfuAM/hYixvISLMEhyBYk7dxrsGJdw15Rq", "99999994", new DateTime(2026, 4, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), 0 },
                    { 5, "012345678", "fatima@mail.com", "Fatima", "Yousef", "$2a$12$ZPzIhfjkDv3uc/4fEkhAfuAM/hYixvISLMEhyBYk7dxrsGJdw15Rq", "99999995", new DateTime(2026, 4, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), 1 }
                });

            migrationBuilder.InsertData(
                table: "ClassroomEquipment",
                columns: new[] { "ClassroomsId", "EquipmentsId" },
                values: new object[,]
                {
                    { 1, 1 },
                    { 2, 2 },
                    { 3, 3 },
                    { 4, 3 },
                    { 5, 4 }
                });

            migrationBuilder.InsertData(
                table: "CourseTrack",
                columns: new[] { "CoursesId", "TracksId" },
                values: new object[,]
                {
                    { 1, 1 },
                    { 3, 2 },
                    { 5, 3 }
                });

            migrationBuilder.InsertData(
                table: "Courses",
                columns: new[] { "Id", "Capacity", "Category", "Description", "Duration", "Fee", "PrerequisiteId", "Title" },
                values: new object[,]
                {
                    { 2, 20, 1, "Advanced concepts in C#", 30, 180.0, 1, "Advanced C#" },
                    { 4, 20, 2, "Working with EF Core", 20, 170.0, 3, "Entity Framework Core" }
                });

            migrationBuilder.InsertData(
                table: "InstructorAvailabilities",
                columns: new[] { "Id", "DayEnd", "DayStart", "EndTime", "InstructorId", "StartTime" },
                values: new object[,]
                {
                    { 1, 4, 0, new TimeOnly(16, 0, 0), 2, new TimeOnly(8, 0, 0) },
                    { 2, 4, 0, new TimeOnly(17, 0, 0), 5, new TimeOnly(9, 0, 0) }
                });

            migrationBuilder.InsertData(
                table: "InstructorExpertises",
                columns: new[] { "CourseId", "InstructorId" },
                values: new object[,]
                {
                    { 1, 2 },
                    { 5, 2 },
                    { 3, 5 }
                });

            migrationBuilder.InsertData(
                table: "Notifications",
                columns: new[] { "Id", "CreatedDate", "Message", "Status", "UserId" },
                values: new object[,]
                {
                    { 1, new DateTime(2026, 4, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), "Welcome to the platform", 0, 1 },
                    { 2, new DateTime(2026, 4, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), "New session assigned", 1, 2 },
                    { 3, new DateTime(2026, 4, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), "New enrollment received", 0, 3 },
                    { 4, new DateTime(2026, 4, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), "Payment reminder", 0, 4 },
                    { 5, new DateTime(2026, 4, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), "Schedule updated", 1, 5 }
                });

            migrationBuilder.InsertData(
                table: "Sessions",
                columns: new[] { "Id", "ClassroomId", "CourseId", "EndTime", "InstructorId", "SessionDate", "StartTime" },
                values: new object[,]
                {
                    { 1, 1, 1, new TimeOnly(11, 0, 0), 2, new DateTime(2026, 4, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeOnly(9, 0, 0) },
                    { 3, 3, 3, new TimeOnly(12, 0, 0), 5, new DateTime(2026, 4, 6, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeOnly(10, 0, 0) },
                    { 5, 5, 5, new TimeOnly(16, 0, 0), 2, new DateTime(2026, 4, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeOnly(14, 0, 0) }
                });

            migrationBuilder.InsertData(
                table: "TraineeCertifications",
                columns: new[] { "Id", "CertificateReferenceNumber", "Status", "TrackId", "TraineeId" },
                values: new object[,]
                {
                    { 1, "CERT-1001", 1, 1, 1 },
                    { 2, "CERT-1002", 1, 2, 4 },
                    { 3, "", 0, 3, 1 },
                    { 4, "", 0, 4, 4 },
                    { 5, "CERT-1005", 1, 5, 1 }
                });

            migrationBuilder.InsertData(
                table: "CourseTrack",
                columns: new[] { "CoursesId", "TracksId" },
                values: new object[,]
                {
                    { 2, 1 },
                    { 4, 2 }
                });

            migrationBuilder.InsertData(
                table: "Enrollments",
                columns: new[] { "Id", "CompletionDate", "EnrollmentDate", "PaymentDueDate", "SessionId", "Status", "TraineeId", "UserId" },
                values: new object[,]
                {
                    { 1, null, new DateTime(2026, 4, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), null, 1, 0, 1, null },
                    { 2, null, new DateTime(2026, 4, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), null, 1, 1, 4, null },
                    { 3, null, new DateTime(2026, 4, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), null, 3, 2, 1, null },
                    { 4, null, new DateTime(2026, 4, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), null, 5, 3, 4, null }
                });

            migrationBuilder.InsertData(
                table: "InstructorExpertises",
                columns: new[] { "CourseId", "InstructorId" },
                values: new object[,]
                {
                    { 2, 2 },
                    { 4, 5 }
                });

            migrationBuilder.InsertData(
                table: "Sessions",
                columns: new[] { "Id", "ClassroomId", "CourseId", "EndTime", "InstructorId", "SessionDate", "StartTime" },
                values: new object[,]
                {
                    { 2, 2, 2, new TimeOnly(14, 0, 0), 2, new DateTime(2026, 4, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeOnly(12, 0, 0) },
                    { 4, 4, 4, new TimeOnly(15, 0, 0), 5, new DateTime(2026, 4, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeOnly(13, 0, 0) }
                });

            migrationBuilder.InsertData(
                table: "Assessments",
                columns: new[] { "Id", "CompletedBy", "DueDate", "EnrollmentId", "Status" },
                values: new object[,]
                {
                    { 1, new DateTime(2026, 4, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 4, 9, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 1 },
                    { 2, null, new DateTime(2026, 4, 9, 0, 0, 0, 0, DateTimeKind.Unspecified), 2, 0 },
                    { 3, new DateTime(2026, 4, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 4, 9, 0, 0, 0, 0, DateTimeKind.Unspecified), 3, 1 },
                    { 4, new DateTime(2026, 4, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 4, 9, 0, 0, 0, 0, DateTimeKind.Unspecified), 4, 1 }
                });

            migrationBuilder.InsertData(
                table: "Balances",
                columns: new[] { "Id", "AmountDue", "DueDate", "EnrollmentId", "Status" },
                values: new object[,]
                {
                    { 1, 0m, new DateTime(2026, 4, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 0 },
                    { 2, 60m, new DateTime(2026, 4, 11, 0, 0, 0, 0, DateTimeKind.Unspecified), 2, 0 },
                    { 3, 0m, new DateTime(2026, 4, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), 3, 0 },
                    { 4, 50m, new DateTime(2026, 4, 9, 0, 0, 0, 0, DateTimeKind.Unspecified), 4, 0 }
                });

            migrationBuilder.InsertData(
                table: "Enrollments",
                columns: new[] { "Id", "CompletionDate", "EnrollmentDate", "PaymentDueDate", "SessionId", "Status", "TraineeId", "UserId" },
                values: new object[,]
                {
                    { 5, null, new DateTime(2026, 4, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), null, 2, 4, 1, null },
                    { 6, null, new DateTime(2026, 4, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), null, 2, 3, 1, null }
                });

            migrationBuilder.InsertData(
                table: "Payments",
                columns: new[] { "Id", "Amount", "EnrollmentId", "PaymentDate", "Status" },
                values: new object[,]
                {
                    { 1, 120m, 1, new DateTime(2026, 4, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), 1 },
                    { 2, 60m, 2, new DateTime(2026, 4, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), 0 },
                    { 3, 150m, 3, new DateTime(2026, 4, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), 1 },
                    { 4, 50m, 4, new DateTime(2026, 4, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), 0 }
                });

            migrationBuilder.InsertData(
                table: "Assessments",
                columns: new[] { "Id", "CompletedBy", "DueDate", "EnrollmentId", "Status" },
                values: new object[,]
                {
                    { 5, null, new DateTime(2026, 4, 9, 0, 0, 0, 0, DateTimeKind.Unspecified), 5, 0 },
                    { 6, new DateTime(2026, 4, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 4, 9, 0, 0, 0, 0, DateTimeKind.Unspecified), 6, 1 }
                });

            migrationBuilder.InsertData(
                table: "Balances",
                columns: new[] { "Id", "AmountDue", "DueDate", "EnrollmentId", "Status" },
                values: new object[] { 5, 0m, new DateTime(2026, 4, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), 5, 0 });

            migrationBuilder.InsertData(
                table: "Payments",
                columns: new[] { "Id", "Amount", "EnrollmentId", "PaymentDate", "Status" },
                values: new object[] { 5, 180m, 5, new DateTime(2026, 4, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), 1 });

            migrationBuilder.CreateIndex(
                name: "IX_Assessments_EnrollmentId",
                table: "Assessments",
                column: "EnrollmentId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Balances_EnrollmentId",
                table: "Balances",
                column: "EnrollmentId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ClassroomEquipment_EquipmentsId",
                table: "ClassroomEquipment",
                column: "EquipmentsId");

            migrationBuilder.CreateIndex(
                name: "IX_Courses_PrerequisiteId",
                table: "Courses",
                column: "PrerequisiteId");

            migrationBuilder.CreateIndex(
                name: "IX_CourseTrack_TracksId",
                table: "CourseTrack",
                column: "TracksId");

            migrationBuilder.CreateIndex(
                name: "IX_Enrollments_SessionId",
                table: "Enrollments",
                column: "SessionId");

            migrationBuilder.CreateIndex(
                name: "IX_Enrollments_TraineeId",
                table: "Enrollments",
                column: "TraineeId");

            migrationBuilder.CreateIndex(
                name: "IX_Enrollments_UserId",
                table: "Enrollments",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_InstructorAvailabilities_InstructorId",
                table: "InstructorAvailabilities",
                column: "InstructorId");

            migrationBuilder.CreateIndex(
                name: "IX_InstructorExpertises_CourseId",
                table: "InstructorExpertises",
                column: "CourseId");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_UserId",
                table: "Notifications",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Payments_EnrollmentId",
                table: "Payments",
                column: "EnrollmentId");

            migrationBuilder.CreateIndex(
                name: "IX_Sessions_ClassroomId_SessionDate",
                table: "Sessions",
                columns: new[] { "ClassroomId", "SessionDate" });

            migrationBuilder.CreateIndex(
                name: "IX_Sessions_CourseId",
                table: "Sessions",
                column: "CourseId");

            migrationBuilder.CreateIndex(
                name: "IX_Sessions_InstructorId_SessionDate",
                table: "Sessions",
                columns: new[] { "InstructorId", "SessionDate" });

            migrationBuilder.CreateIndex(
                name: "IX_TraineeCertifications_TrackId",
                table: "TraineeCertifications",
                column: "TrackId");

            migrationBuilder.CreateIndex(
                name: "IX_TraineeCertifications_TraineeId",
                table: "TraineeCertifications",
                column: "TraineeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Assessments");

            migrationBuilder.DropTable(
                name: "Balances");

            migrationBuilder.DropTable(
                name: "ClassroomEquipment");

            migrationBuilder.DropTable(
                name: "CourseTrack");

            migrationBuilder.DropTable(
                name: "InstructorAvailabilities");

            migrationBuilder.DropTable(
                name: "InstructorExpertises");

            migrationBuilder.DropTable(
                name: "Notifications");

            migrationBuilder.DropTable(
                name: "Payments");

            migrationBuilder.DropTable(
                name: "TraineeCertifications");

            migrationBuilder.DropTable(
                name: "Equipments");

            migrationBuilder.DropTable(
                name: "Enrollments");

            migrationBuilder.DropTable(
                name: "Tracks");

            migrationBuilder.DropTable(
                name: "Sessions");

            migrationBuilder.DropTable(
                name: "Classrooms");

            migrationBuilder.DropTable(
                name: "Courses");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
