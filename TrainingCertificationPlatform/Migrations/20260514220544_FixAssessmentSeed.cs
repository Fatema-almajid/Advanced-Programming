using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace TrainingCertificationPlatform.Migrations
{
    /// <inheritdoc />
    public partial class FixAssessmentSeed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 100);

            migrationBuilder.UpdateData(
                table: "Assessments",
                keyColumn: "Id",
                keyValue: 1,
                column: "DueDate",
                value: new DateTime(2026, 4, 9, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.UpdateData(
                table: "Balances",
                keyColumn: "Id",
                keyValue: 1,
                column: "AmountDue",
                value: 0m);

            migrationBuilder.InsertData(
                table: "Classrooms",
                columns: new[] { "Id", "Name", "Seats" },
                values: new object[,]
                {
                    { 2, "Room B", 25 },
                    { 3, "Lab 1", 20 },
                    { 4, "Lab 2", 20 },
                    { 5, "Conference Hall", 50 }
                });

            migrationBuilder.UpdateData(
                table: "Courses",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Capacity", "Category", "Description", "Duration", "Fee" },
                values: new object[] { 25, 1, "Introduction to C# programming", 20, 120.0 });

            migrationBuilder.UpdateData(
                table: "Courses",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Capacity", "Description", "Duration", "Fee" },
                values: new object[] { 20, "Advanced concepts in C#", 30, 180.0 });

            migrationBuilder.InsertData(
                table: "Courses",
                columns: new[] { "Id", "Capacity", "Category", "Description", "Duration", "Fee", "PrerequisiteId", "Title" },
                values: new object[,]
                {
                    { 3, 30, 2, "Introduction to SQL Server", 25, 150.0, null, "SQL Fundamentals" },
                    { 5, 25, 4, "Introduction to networking", 15, 100.0, null, "Networking Basics" }
                });

            migrationBuilder.InsertData(
                table: "Equipments",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 2, "Whiteboard" },
                    { 3, "Lab Computers" },
                    { 4, "Microphones" },
                    { 5, "Networking Kit" }
                });

            migrationBuilder.UpdateData(
                table: "InstructorAvailabilities",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "EndTime", "StartTime" },
                values: new object[] { new TimeOnly(16, 0, 0), new TimeOnly(8, 0, 0) });

            migrationBuilder.InsertData(
                table: "InstructorAvailabilities",
                columns: new[] { "Id", "DayEnd", "DayStart", "EndTime", "InstructorId", "StartTime" },
                values: new object[,]
                {
                    { 3, 3, 1, new TimeOnly(18, 0, 0), 2, new TimeOnly(10, 0, 0) },
                    { 5, 6, 6, new TimeOnly(13, 0, 0), 2, new TimeOnly(9, 0, 0) }
                });

            migrationBuilder.UpdateData(
                table: "Notifications",
                keyColumn: "Id",
                keyValue: 1,
                column: "Message",
                value: "Welcome to the platform");

            migrationBuilder.InsertData(
                table: "Notifications",
                columns: new[] { "Id", "CreatedDate", "Message", "Status", "UserId" },
                values: new object[] { 2, new DateTime(2026, 4, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), "New session assigned", 1, 2 });

            migrationBuilder.UpdateData(
                table: "Payments",
                keyColumn: "Id",
                keyValue: 1,
                column: "Amount",
                value: 120m);

            migrationBuilder.UpdateData(
                table: "Sessions",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "EndTime", "StartTime" },
                values: new object[] { new TimeOnly(11, 0, 0), new TimeOnly(9, 0, 0) });

            migrationBuilder.UpdateData(
                table: "Tracks",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Description", "Name" },
                values: new object[] { "Backend programming track", "Backend Development" });

            migrationBuilder.InsertData(
                table: "Tracks",
                columns: new[] { "Id", "Description", "Name" },
                values: new object[,]
                {
                    { 2, "Database management track", "Database Administration" },
                    { 3, "Networking certification track", "Networking Essentials" },
                    { 4, "Complete web development track", "Full Stack Development" },
                    { 5, "Software engineering foundations", "Software Engineering" }
                });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Password", "Phone" },
                values: new object[] { "$2a$11$x4qb8776Dbr0Ltc2M/tM/O5kFl0tzNF1agUUs543BeGCMz/K/vaT.", "99999991" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Password", "Phone" },
                values: new object[] { "$2a$11$wfxwG1i.WiTjstYbx6pE3ONCKTQYKSqSynJQiCuT1YI2CSjL8Jrsq", "99999992" });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "Email", "FirstName", "LastName", "Password", "Phone", "RegistrationDate", "Role" },
                values: new object[,]
                {
                    { 3, "mariam@mail.com", "Mariam", "Hassan", "$2a$11$YvIaDgzxV1IRx9uX6cegOenui7ECXHkzuh/SjzbuYr7kut6IMXg..", "99999993", new DateTime(2026, 4, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), 2 },
                    { 4, "omar@mail.com", "Omar", "Ali", "$2a$11$SCuxvvUXWltUe7Hz3WGQN.m2LwyW4nCGeZbEmC/ioPGfb9rVA9wbm", "99999994", new DateTime(2026, 4, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), 0 },
                    { 5, "fatima@mail.com", "Fatima", "Yousef", "$2a$11$sRMBUwJ.sjwigPf16zqWIeZpuyvn.t4TBM0oSNvpbRr/w6ovF0uTO", "99999995", new DateTime(2026, 4, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), 1 }
                });

            migrationBuilder.InsertData(
                table: "ClassroomEquipment",
                columns: new[] { "ClassroomsId", "EquipmentsId" },
                values: new object[,]
                {
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
                    { 3, 2 },
                    { 5, 3 }
                });

            migrationBuilder.InsertData(
                table: "Courses",
                columns: new[] { "Id", "Capacity", "Category", "Description", "Duration", "Fee", "PrerequisiteId", "Title" },
                values: new object[] { 4, 20, 2, "Working with EF Core", 20, 170.0, 3, "Entity Framework Core" });

            migrationBuilder.InsertData(
                table: "Enrollments",
                columns: new[] { "Id", "CompletionDate", "EnrollmentDate", "PaymentDueDate", "SessionId", "Status", "TraineeId", "UserId" },
                values: new object[] { 2, null, new DateTime(2026, 4, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), null, 1, 1, 4, null });

            migrationBuilder.InsertData(
                table: "InstructorAvailabilities",
                columns: new[] { "Id", "DayEnd", "DayStart", "EndTime", "InstructorId", "StartTime" },
                values: new object[,]
                {
                    { 2, 4, 0, new TimeOnly(17, 0, 0), 5, new TimeOnly(9, 0, 0) },
                    { 4, 4, 2, new TimeOnly(19, 0, 0), 5, new TimeOnly(11, 0, 0) }
                });

            migrationBuilder.InsertData(
                table: "InstructorExpertises",
                columns: new[] { "CourseId", "InstructorId" },
                values: new object[,]
                {
                    { 5, 2 },
                    { 3, 5 }
                });

            migrationBuilder.InsertData(
                table: "Notifications",
                columns: new[] { "Id", "CreatedDate", "Message", "Status", "UserId" },
                values: new object[,]
                {
                    { 3, new DateTime(2026, 4, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), "New enrollment received", 0, 3 },
                    { 4, new DateTime(2026, 4, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), "Payment reminder", 0, 4 },
                    { 5, new DateTime(2026, 4, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), "Schedule updated", 1, 5 }
                });

            migrationBuilder.InsertData(
                table: "Sessions",
                columns: new[] { "Id", "ClassroomId", "CourseId", "EndTime", "InstructorId", "SessionDate", "StartTime" },
                values: new object[,]
                {
                    { 2, 2, 2, new TimeOnly(14, 0, 0), 2, new DateTime(2026, 4, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeOnly(12, 0, 0) },
                    { 3, 3, 3, new TimeOnly(12, 0, 0), 5, new DateTime(2026, 4, 6, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeOnly(10, 0, 0) },
                    { 5, 5, 5, new TimeOnly(16, 0, 0), 2, new DateTime(2026, 4, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeOnly(14, 0, 0) }
                });

            migrationBuilder.InsertData(
                table: "TraineeCertifications",
                columns: new[] { "Id", "Status", "TrackId", "TraineeId" },
                values: new object[,]
                {
                    { 2, 1, 2, 4 },
                    { 3, 0, 3, 1 },
                    { 4, 0, 4, 4 },
                    { 5, 1, 5, 1 }
                });

            migrationBuilder.InsertData(
                table: "Assessments",
                columns: new[] { "Id", "CompletedBy", "DueDate", "EnrollmentId", "Status" },
                values: new object[] { 2, null, new DateTime(2026, 4, 9, 0, 0, 0, 0, DateTimeKind.Unspecified), 2, 0 });

            migrationBuilder.InsertData(
                table: "Balances",
                columns: new[] { "Id", "AmountDue", "DueDate", "EnrollmentId", "Status" },
                values: new object[] { 2, 60m, new DateTime(2026, 4, 11, 0, 0, 0, 0, DateTimeKind.Unspecified), 2, 0 });

            migrationBuilder.InsertData(
                table: "CourseTrack",
                columns: new[] { "CoursesId", "TracksId" },
                values: new object[] { 4, 2 });

            migrationBuilder.InsertData(
                table: "Enrollments",
                columns: new[] { "Id", "CompletionDate", "EnrollmentDate", "PaymentDueDate", "SessionId", "Status", "TraineeId", "UserId" },
                values: new object[,]
                {
                    { 3, null, new DateTime(2026, 4, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), null, 3, 2, 1, null },
                    { 4, null, new DateTime(2026, 4, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), null, 5, 3, 4, null },
                    { 5, null, new DateTime(2026, 4, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), null, 2, 4, 1, null }
                });

            migrationBuilder.InsertData(
                table: "InstructorExpertises",
                columns: new[] { "CourseId", "InstructorId" },
                values: new object[] { 4, 5 });

            migrationBuilder.InsertData(
                table: "Payments",
                columns: new[] { "Id", "Amount", "EnrollmentId", "PaymentDate", "Status" },
                values: new object[] { 2, 60m, 2, new DateTime(2026, 4, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), 0 });

            migrationBuilder.InsertData(
                table: "Sessions",
                columns: new[] { "Id", "ClassroomId", "CourseId", "EndTime", "InstructorId", "SessionDate", "StartTime" },
                values: new object[] { 4, 4, 4, new TimeOnly(15, 0, 0), 5, new DateTime(2026, 4, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeOnly(13, 0, 0) });

            migrationBuilder.InsertData(
                table: "Assessments",
                columns: new[] { "Id", "CompletedBy", "DueDate", "EnrollmentId", "Status" },
                values: new object[,]
                {
                    { 3, new DateTime(2026, 4, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 4, 9, 0, 0, 0, 0, DateTimeKind.Unspecified), 3, 1 },
                    { 4, new DateTime(2026, 4, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 4, 9, 0, 0, 0, 0, DateTimeKind.Unspecified), 4, 1 },
                    { 5, null, new DateTime(2026, 4, 9, 0, 0, 0, 0, DateTimeKind.Unspecified), 5, 0 }
                });

            migrationBuilder.InsertData(
                table: "Balances",
                columns: new[] { "Id", "AmountDue", "DueDate", "EnrollmentId", "Status" },
                values: new object[,]
                {
                    { 3, 0m, new DateTime(2026, 4, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), 3, 0 },
                    { 4, 50m, new DateTime(2026, 4, 9, 0, 0, 0, 0, DateTimeKind.Unspecified), 4, 0 },
                    { 5, 0m, new DateTime(2026, 4, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), 5, 0 }
                });

            migrationBuilder.InsertData(
                table: "Payments",
                columns: new[] { "Id", "Amount", "EnrollmentId", "PaymentDate", "Status" },
                values: new object[,]
                {
                    { 3, 150m, 3, new DateTime(2026, 4, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), 1 },
                    { 4, 50m, 4, new DateTime(2026, 4, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), 0 },
                    { 5, 180m, 5, new DateTime(2026, 4, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), 1 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Assessments",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Assessments",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Assessments",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Assessments",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Balances",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Balances",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Balances",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Balances",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "ClassroomEquipment",
                keyColumns: new[] { "ClassroomsId", "EquipmentsId" },
                keyValues: new object[] { 2, 2 });

            migrationBuilder.DeleteData(
                table: "ClassroomEquipment",
                keyColumns: new[] { "ClassroomsId", "EquipmentsId" },
                keyValues: new object[] { 3, 3 });

            migrationBuilder.DeleteData(
                table: "ClassroomEquipment",
                keyColumns: new[] { "ClassroomsId", "EquipmentsId" },
                keyValues: new object[] { 4, 3 });

            migrationBuilder.DeleteData(
                table: "ClassroomEquipment",
                keyColumns: new[] { "ClassroomsId", "EquipmentsId" },
                keyValues: new object[] { 5, 4 });

            migrationBuilder.DeleteData(
                table: "CourseTrack",
                keyColumns: new[] { "CoursesId", "TracksId" },
                keyValues: new object[] { 3, 2 });

            migrationBuilder.DeleteData(
                table: "CourseTrack",
                keyColumns: new[] { "CoursesId", "TracksId" },
                keyValues: new object[] { 4, 2 });

            migrationBuilder.DeleteData(
                table: "CourseTrack",
                keyColumns: new[] { "CoursesId", "TracksId" },
                keyValues: new object[] { 5, 3 });

            migrationBuilder.DeleteData(
                table: "Equipments",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "InstructorAvailabilities",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "InstructorAvailabilities",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "InstructorAvailabilities",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "InstructorAvailabilities",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "InstructorExpertises",
                keyColumns: new[] { "CourseId", "InstructorId" },
                keyValues: new object[] { 5, 2 });

            migrationBuilder.DeleteData(
                table: "InstructorExpertises",
                keyColumns: new[] { "CourseId", "InstructorId" },
                keyValues: new object[] { 3, 5 });

            migrationBuilder.DeleteData(
                table: "InstructorExpertises",
                keyColumns: new[] { "CourseId", "InstructorId" },
                keyValues: new object[] { 4, 5 });

            migrationBuilder.DeleteData(
                table: "Notifications",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Notifications",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Notifications",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Notifications",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Payments",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Payments",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Payments",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Payments",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Sessions",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "TraineeCertifications",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "TraineeCertifications",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "TraineeCertifications",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "TraineeCertifications",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Classrooms",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Courses",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Enrollments",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Enrollments",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Enrollments",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Enrollments",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Equipments",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Equipments",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Equipments",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Tracks",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Tracks",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Tracks",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Tracks",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Sessions",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Sessions",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Sessions",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Classrooms",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Classrooms",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Classrooms",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Courses",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Courses",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.UpdateData(
                table: "Assessments",
                keyColumn: "Id",
                keyValue: 1,
                column: "DueDate",
                value: new DateTime(2026, 4, 4, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.UpdateData(
                table: "Balances",
                keyColumn: "Id",
                keyValue: 1,
                column: "AmountDue",
                value: 50m);

            migrationBuilder.UpdateData(
                table: "Courses",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Capacity", "Category", "Description", "Duration", "Fee" },
                values: new object[] { 30, 2, "Intro", 10, 100.0 });

            migrationBuilder.UpdateData(
                table: "Courses",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Capacity", "Description", "Duration", "Fee" },
                values: new object[] { 25, "Advanced", 15, 150.0 });

            migrationBuilder.UpdateData(
                table: "InstructorAvailabilities",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "EndTime", "StartTime" },
                values: new object[] { new TimeOnly(17, 0, 0), new TimeOnly(9, 0, 0) });

            migrationBuilder.UpdateData(
                table: "Notifications",
                keyColumn: "Id",
                keyValue: 1,
                column: "Message",
                value: "Welcome");

            migrationBuilder.UpdateData(
                table: "Payments",
                keyColumn: "Id",
                keyValue: 1,
                column: "Amount",
                value: 100m);

            migrationBuilder.UpdateData(
                table: "Sessions",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "EndTime", "StartTime" },
                values: new object[] { new TimeOnly(12, 0, 0), new TimeOnly(10, 0, 0) });

            migrationBuilder.UpdateData(
                table: "Tracks",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Description", "Name" },
                values: new object[] { "Programming Track", "Backend" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Password", "Phone" },
                values: new object[] { "$2a$11$examplehash...", "99999999" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Password", "Phone" },
                values: new object[] { "$2a$11$examplehash...", "88888888" });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "Email", "FirstName", "LastName", "Password", "Phone", "RegistrationDate", "Role" },
                values: new object[] { 100, "coordinator@mail.com", "Mariam", "Coordinator", "123456", "77777777", new DateTime(2026, 4, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), 2 });
        }
    }
}
