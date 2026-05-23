using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TrainingCertificationPlatform.Migrations
{
    /// <inheritdoc />
    public partial class FixedData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Assessments",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "TraineeCertifications",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.UpdateData(
                table: "Assessments",
                keyColumn: "Id",
                keyValue: 1,
                column: "CompletedBy",
                value: new DateTime(2026, 6, 22, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.UpdateData(
                table: "Balances",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "AmountDue", "DueDate" },
                values: new object[] { 0m, new DateTime(2026, 6, 23, 0, 0, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.UpdateData(
                table: "Enrollments",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CompletionDate", "Status" },
                values: new object[] { new DateTime(2026, 6, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), 3 });

            migrationBuilder.UpdateData(
                table: "Enrollments",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "EnrollmentDate", "SessionId", "Status" },
                values: new object[] { new DateTime(2026, 6, 23, 0, 0, 0, 0, DateTimeKind.Unspecified), 3, 1 });

            migrationBuilder.InsertData(
                table: "Enrollments",
                columns: new[] { "Id", "CompletionDate", "EnrollmentDate", "PaymentDueDate", "SessionId", "Status", "TraineeId", "UserId" },
                values: new object[] { 3, null, new DateTime(2026, 6, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), null, 2, 4, 1, null });

            migrationBuilder.InsertData(
                table: "Feedbacks",
                columns: new[] { "Id", "Comment", "ContentRating", "CourseId", "InstructorId", "InstructorRating", "OrganizationRating", "Rating", "RecommendCourse", "SubmittedAt", "TraineeId" },
                values: new object[] { 1, "Excellent instructor and very clear explanations", 5, 1, 2, 5, 4, 5, true, new DateTime(2026, 6, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), 1 });

            migrationBuilder.UpdateData(
                table: "Payments",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Amount", "PaymentDate", "Status" },
                values: new object[] { 180m, new DateTime(2026, 6, 23, 0, 0, 0, 0, DateTimeKind.Unspecified), 1 });

            migrationBuilder.UpdateData(
                table: "Sessions",
                keyColumn: "Id",
                keyValue: 2,
                column: "CourseId",
                value: 3);

            migrationBuilder.UpdateData(
                table: "Sessions",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CourseId", "SessionDate" },
                values: new object[] { 2, new DateTime(2026, 6, 25, 0, 0, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.UpdateData(
                table: "Sessions",
                keyColumn: "Id",
                keyValue: 4,
                column: "SessionDate",
                value: new DateTime(2026, 6, 25, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.InsertData(
                table: "Balances",
                columns: new[] { "Id", "AmountDue", "DueDate", "EnrollmentId", "Status" },
                values: new object[] { 3, 90m, new DateTime(2026, 6, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), 3, 0 });

            migrationBuilder.InsertData(
                table: "Payments",
                columns: new[] { "Id", "Amount", "EnrollmentId", "PaymentDate", "Status" },
                values: new object[] { 3, 60m, 3, new DateTime(2026, 6, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), 0 });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Balances",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Feedbacks",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Payments",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Enrollments",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.UpdateData(
                table: "Assessments",
                keyColumn: "Id",
                keyValue: 1,
                column: "CompletedBy",
                value: new DateTime(2026, 6, 21, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.InsertData(
                table: "Assessments",
                columns: new[] { "Id", "CompletedBy", "DueDate", "EnrollmentId", "Status" },
                values: new object[] { 2, new DateTime(2026, 6, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 6, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), 2, 2 });

            migrationBuilder.UpdateData(
                table: "Balances",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "AmountDue", "DueDate" },
                values: new object[] { 120m, new DateTime(2026, 6, 22, 0, 0, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.UpdateData(
                table: "Enrollments",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CompletionDate", "Status" },
                values: new object[] { null, 1 });

            migrationBuilder.UpdateData(
                table: "Enrollments",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "EnrollmentDate", "SessionId", "Status" },
                values: new object[] { new DateTime(2026, 6, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), 2, 4 });

            migrationBuilder.UpdateData(
                table: "Payments",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Amount", "PaymentDate", "Status" },
                values: new object[] { 60m, new DateTime(2026, 6, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), 0 });

            migrationBuilder.UpdateData(
                table: "Sessions",
                keyColumn: "Id",
                keyValue: 2,
                column: "CourseId",
                value: 2);

            migrationBuilder.UpdateData(
                table: "Sessions",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CourseId", "SessionDate" },
                values: new object[] { 3, new DateTime(2026, 6, 22, 0, 0, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.UpdateData(
                table: "Sessions",
                keyColumn: "Id",
                keyValue: 4,
                column: "SessionDate",
                value: new DateTime(2026, 6, 22, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.InsertData(
                table: "TraineeCertifications",
                columns: new[] { "Id", "CertificateReferenceNumber", "Status", "TrackId", "TraineeId" },
                values: new object[] { 2, "CERT-1002", 0, 2, 1 });
        }
    }
}
