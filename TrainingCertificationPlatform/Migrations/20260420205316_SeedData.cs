using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace TrainingCertificationPlatform.Migrations
{
    /// <inheritdoc />
    public partial class SeedData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Classrooms",
                columns: new[] { "Id", "Name", "Seats" },
                values: new object[] { 1, "Room A", 30 });

            migrationBuilder.InsertData(
                table: "Courses",
                columns: new[] { "Id", "Capacity", "Category", "Description", "Duration", "Fee", "PrerequisiteId", "Title" },
                values: new object[] { 1, 30, 0, "Intro", 10, 100.0, null, "C# Basics" });

            migrationBuilder.InsertData(
                table: "Equipments",
                columns: new[] { "Id", "Name" },
                values: new object[] { 1, "Projector" });

            migrationBuilder.InsertData(
                table: "Tracks",
                columns: new[] { "Id", "Description", "Name" },
                values: new object[] { 1, "Programming Track", "Backend" });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "Email", "FirstName", "LastName", "Password", "Phone", "RegistrationDate", "Role" },
                values: new object[,]
                {
                    { 1, "ali@mail.com", "Ali", "Ahmad", "123", "99999999", new DateTime(2026, 4, 20, 23, 53, 15, 885, DateTimeKind.Local).AddTicks(3120), 0 },
                    { 2, "sara@mail.com", "Sara", "Mohamed", "123", "88888888", new DateTime(2026, 4, 20, 23, 53, 15, 887, DateTimeKind.Local).AddTicks(5658), 1 }
                });

            migrationBuilder.InsertData(
                table: "ClassroomEquipment",
                columns: new[] { "ClassroomsId", "EquipmentsId" },
                values: new object[] { 1, 1 });

            migrationBuilder.InsertData(
                table: "CourseTrack",
                columns: new[] { "CoursesId", "TracksId" },
                values: new object[] { 1, 1 });

            migrationBuilder.InsertData(
                table: "Courses",
                columns: new[] { "Id", "Capacity", "Category", "Description", "Duration", "Fee", "PrerequisiteId", "Title" },
                values: new object[] { 2, 25, 0, "Advanced", 15, 150.0, 1, "Advanced C#" });

            migrationBuilder.InsertData(
                table: "InstructorAvailabilities",
                columns: new[] { "Id", "DayEnd", "DayStart", "EndTime", "InstructorId", "StartTime" },
                values: new object[] { 1, 4, 0, new TimeOnly(17, 0, 0), 2, new TimeOnly(9, 0, 0) });

            migrationBuilder.InsertData(
                table: "InstructorExpertises",
                columns: new[] { "CourseId", "InstructorId" },
                values: new object[] { 1, 2 });

            migrationBuilder.InsertData(
                table: "Notifications",
                columns: new[] { "Id", "CreatedDate", "Message", "Status", "UserId" },
                values: new object[] { 1, new DateTime(2026, 4, 20, 23, 53, 15, 890, DateTimeKind.Local).AddTicks(1373), "Welcome", 0, 1 });

            migrationBuilder.InsertData(
                table: "Sessions",
                columns: new[] { "Id", "ClassroomId", "CourseId", "EndTime", "InstructorId", "SessionDate", "StartTime" },
                values: new object[] { 1, 1, 1, new TimeOnly(12, 0, 0), 2, new DateTime(2026, 4, 20, 23, 53, 15, 889, DateTimeKind.Local).AddTicks(1832), new TimeOnly(10, 0, 0) });

            migrationBuilder.InsertData(
                table: "TraineeCertifications",
                columns: new[] { "Id", "Status", "TrackId", "TraineeId" },
                values: new object[] { 1, 1, 1, 1 });

            migrationBuilder.InsertData(
                table: "CourseTrack",
                columns: new[] { "CoursesId", "TracksId" },
                values: new object[] { 2, 1 });

            migrationBuilder.InsertData(
                table: "Enrollments",
                columns: new[] { "Id", "CompletionDate", "EnrollmentDate", "PaymentDueDate", "SessionId", "Status", "TraineeId" },
                values: new object[] { 1, null, new DateTime(2026, 4, 20, 23, 53, 15, 889, DateTimeKind.Local).AddTicks(4345), null, 1, 0, 1 });

            migrationBuilder.InsertData(
                table: "InstructorExpertises",
                columns: new[] { "CourseId", "InstructorId" },
                values: new object[] { 2, 2 });

            migrationBuilder.InsertData(
                table: "Assessments",
                columns: new[] { "Id", "CompletedBy", "DueDate", "EnrollmentId", "Status" },
                values: new object[] { 1, null, new DateTime(2026, 4, 20, 23, 53, 15, 889, DateTimeKind.Local).AddTicks(9632), 1, 0 });

            migrationBuilder.InsertData(
                table: "Balances",
                columns: new[] { "Id", "AmountDue", "DueDate", "EnrollmentId" },
                values: new object[] { 1, 50, new DateTime(2026, 4, 20, 23, 53, 15, 889, DateTimeKind.Local).AddTicks(7997), 1 });

            migrationBuilder.InsertData(
                table: "Payments",
                columns: new[] { "Id", "Amount", "EnrollmentId", "PaymentDate", "Status" },
                values: new object[] { 1, 100.0, 1, new DateTime(2026, 4, 20, 23, 53, 15, 889, DateTimeKind.Local).AddTicks(6096), 1 });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Assessments",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Balances",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "ClassroomEquipment",
                keyColumns: new[] { "ClassroomsId", "EquipmentsId" },
                keyValues: new object[] { 1, 1 });

            migrationBuilder.DeleteData(
                table: "CourseTrack",
                keyColumns: new[] { "CoursesId", "TracksId" },
                keyValues: new object[] { 1, 1 });

            migrationBuilder.DeleteData(
                table: "CourseTrack",
                keyColumns: new[] { "CoursesId", "TracksId" },
                keyValues: new object[] { 2, 1 });

            migrationBuilder.DeleteData(
                table: "InstructorAvailabilities",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "InstructorExpertises",
                keyColumns: new[] { "CourseId", "InstructorId" },
                keyValues: new object[] { 1, 2 });

            migrationBuilder.DeleteData(
                table: "InstructorExpertises",
                keyColumns: new[] { "CourseId", "InstructorId" },
                keyValues: new object[] { 2, 2 });

            migrationBuilder.DeleteData(
                table: "Notifications",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Payments",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "TraineeCertifications",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Courses",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Enrollments",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Equipments",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Tracks",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Sessions",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Classrooms",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Courses",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2);
        }
    }
}
