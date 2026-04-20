using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TrainingCertificationPlatform.Migrations
{
    /// <inheritdoc />
    public partial class FinalFixed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InstructorExpertises_Users_InstructorId",
                table: "InstructorExpertises");

            migrationBuilder.DropForeignKey(
                name: "FK_Sessions_Users_InstructorId",
                table: "Sessions");

            migrationBuilder.DropIndex(
                name: "IX_Balances_EnrollmentId",
                table: "Balances");

            migrationBuilder.DropIndex(
                name: "IX_Assessments_EnrollmentId",
                table: "Assessments");

            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "Enrollments",
                type: "int",
                nullable: true);

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
                column: "DueDate",
                value: new DateTime(2026, 4, 4, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.UpdateData(
                table: "Enrollments",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "EnrollmentDate", "UserId" },
                values: new object[] { new DateTime(2026, 4, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), null });

            migrationBuilder.UpdateData(
                table: "Notifications",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedDate",
                value: new DateTime(2026, 4, 4, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.UpdateData(
                table: "Payments",
                keyColumn: "Id",
                keyValue: 1,
                column: "PaymentDate",
                value: new DateTime(2026, 4, 4, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.UpdateData(
                table: "Sessions",
                keyColumn: "Id",
                keyValue: 1,
                column: "SessionDate",
                value: new DateTime(2026, 4, 4, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Password", "RegistrationDate" },
                values: new object[] { "$2a$11$examplehash...", new DateTime(2026, 4, 4, 0, 0, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Password", "RegistrationDate" },
                values: new object[] { "$2a$11$examplehash...", new DateTime(2026, 4, 4, 0, 0, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.CreateIndex(
                name: "IX_Enrollments_TraineeId",
                table: "Enrollments",
                column: "TraineeId");

            migrationBuilder.CreateIndex(
                name: "IX_Enrollments_UserId",
                table: "Enrollments",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Balances_EnrollmentId",
                table: "Balances",
                column: "EnrollmentId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Assessments_EnrollmentId",
                table: "Assessments",
                column: "EnrollmentId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Enrollments_Users_TraineeId",
                table: "Enrollments",
                column: "TraineeId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Enrollments_Users_UserId",
                table: "Enrollments",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_InstructorExpertises_Users_InstructorId",
                table: "InstructorExpertises",
                column: "InstructorId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Sessions_Users_InstructorId",
                table: "Sessions",
                column: "InstructorId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Enrollments_Users_TraineeId",
                table: "Enrollments");

            migrationBuilder.DropForeignKey(
                name: "FK_Enrollments_Users_UserId",
                table: "Enrollments");

            migrationBuilder.DropForeignKey(
                name: "FK_InstructorExpertises_Users_InstructorId",
                table: "InstructorExpertises");

            migrationBuilder.DropForeignKey(
                name: "FK_Sessions_Users_InstructorId",
                table: "Sessions");

            migrationBuilder.DropIndex(
                name: "IX_Enrollments_TraineeId",
                table: "Enrollments");

            migrationBuilder.DropIndex(
                name: "IX_Enrollments_UserId",
                table: "Enrollments");

            migrationBuilder.DropIndex(
                name: "IX_Balances_EnrollmentId",
                table: "Balances");

            migrationBuilder.DropIndex(
                name: "IX_Assessments_EnrollmentId",
                table: "Assessments");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Enrollments");

            migrationBuilder.UpdateData(
                table: "Assessments",
                keyColumn: "Id",
                keyValue: 1,
                column: "DueDate",
                value: new DateTime(2026, 4, 20, 23, 53, 15, 889, DateTimeKind.Local).AddTicks(9632));

            migrationBuilder.UpdateData(
                table: "Balances",
                keyColumn: "Id",
                keyValue: 1,
                column: "DueDate",
                value: new DateTime(2026, 4, 20, 23, 53, 15, 889, DateTimeKind.Local).AddTicks(7997));

            migrationBuilder.UpdateData(
                table: "Enrollments",
                keyColumn: "Id",
                keyValue: 1,
                column: "EnrollmentDate",
                value: new DateTime(2026, 4, 20, 23, 53, 15, 889, DateTimeKind.Local).AddTicks(4345));

            migrationBuilder.UpdateData(
                table: "Notifications",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedDate",
                value: new DateTime(2026, 4, 20, 23, 53, 15, 890, DateTimeKind.Local).AddTicks(1373));

            migrationBuilder.UpdateData(
                table: "Payments",
                keyColumn: "Id",
                keyValue: 1,
                column: "PaymentDate",
                value: new DateTime(2026, 4, 20, 23, 53, 15, 889, DateTimeKind.Local).AddTicks(6096));

            migrationBuilder.UpdateData(
                table: "Sessions",
                keyColumn: "Id",
                keyValue: 1,
                column: "SessionDate",
                value: new DateTime(2026, 4, 20, 23, 53, 15, 889, DateTimeKind.Local).AddTicks(1832));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Password", "RegistrationDate" },
                values: new object[] { "123", new DateTime(2026, 4, 20, 23, 53, 15, 885, DateTimeKind.Local).AddTicks(3120) });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Password", "RegistrationDate" },
                values: new object[] { "123", new DateTime(2026, 4, 20, 23, 53, 15, 887, DateTimeKind.Local).AddTicks(5658) });

            migrationBuilder.CreateIndex(
                name: "IX_Balances_EnrollmentId",
                table: "Balances",
                column: "EnrollmentId");

            migrationBuilder.CreateIndex(
                name: "IX_Assessments_EnrollmentId",
                table: "Assessments",
                column: "EnrollmentId");

            migrationBuilder.AddForeignKey(
                name: "FK_InstructorExpertises_Users_InstructorId",
                table: "InstructorExpertises",
                column: "InstructorId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Sessions_Users_InstructorId",
                table: "Sessions",
                column: "InstructorId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
