using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TrainingCertificationPlatform.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedSeedData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Assessments",
                columns: new[] { "Id", "CompletedBy", "DueDate", "EnrollmentId", "Status" },
                values: new object[] { 2, new DateTime(2026, 6, 29, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 6, 27, 0, 0, 0, 0, DateTimeKind.Unspecified), 2, 1 });

            migrationBuilder.UpdateData(
                table: "Enrollments",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CompletionDate", "Status" },
                values: new object[] { new DateTime(2026, 6, 29, 0, 0, 0, 0, DateTimeKind.Unspecified), 3 });

            migrationBuilder.InsertData(
                table: "Feedbacks",
                columns: new[] { "Id", "Comment", "ContentRating", "CourseId", "InstructorId", "InstructorRating", "OrganizationRating", "Rating", "RecommendCourse", "SubmittedAt", "TraineeId" },
                values: new object[] { 2, "Challenging course but very useful", 4, 2, 2, 5, 4, 4, true, new DateTime(2026, 6, 29, 0, 0, 0, 0, DateTimeKind.Unspecified), 1 });

            migrationBuilder.UpdateData(
                table: "Notifications",
                keyColumn: "Id",
                keyValue: 2,
                column: "Message",
                value: "You have upcoming assigned sessions");

            migrationBuilder.InsertData(
                table: "Notifications",
                columns: new[] { "Id", "CreatedDate", "Message", "Status", "UserId" },
                values: new object[] { 3, new DateTime(2026, 6, 29, 0, 0, 0, 0, DateTimeKind.Unspecified), "Congratulations, you have completed the Backend Development track", 0, 1 });

            migrationBuilder.UpdateData(
                table: "Sessions",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "EndTime", "StartTime" },
                values: new object[] { new TimeOnly(14, 0, 0), new TimeOnly(12, 0, 0) });

            migrationBuilder.UpdateData(
                table: "Sessions",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "EndTime", "StartTime" },
                values: new object[] { new TimeOnly(16, 0, 0), new TimeOnly(14, 0, 0) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Assessments",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Feedbacks",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Notifications",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.UpdateData(
                table: "Enrollments",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CompletionDate", "Status" },
                values: new object[] { null, 1 });

            migrationBuilder.UpdateData(
                table: "Notifications",
                keyColumn: "Id",
                keyValue: 2,
                column: "Message",
                value: "You have a scheduling conflict");

            migrationBuilder.UpdateData(
                table: "Sessions",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "EndTime", "StartTime" },
                values: new object[] { new TimeOnly(11, 0, 0), new TimeOnly(9, 0, 0) });

            migrationBuilder.UpdateData(
                table: "Sessions",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "EndTime", "StartTime" },
                values: new object[] { new TimeOnly(12, 0, 0), new TimeOnly(10, 0, 0) });
        }
    }
}
