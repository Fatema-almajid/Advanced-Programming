using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TrainingCertificationPlatform.Migrations
{
    /// <inheritdoc />
    public partial class LinkEnrollmentToBalanceAndPayments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "EnrollmentId1",
                table: "Payments",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "EnrollmentId1",
                table: "Balances",
                type: "int",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Balances",
                keyColumn: "Id",
                keyValue: 1,
                column: "EnrollmentId1",
                value: null);

            migrationBuilder.UpdateData(
                table: "Payments",
                keyColumn: "Id",
                keyValue: 1,
                column: "EnrollmentId1",
                value: null);

            migrationBuilder.CreateIndex(
                name: "IX_Payments_EnrollmentId1",
                table: "Payments",
                column: "EnrollmentId1");

            migrationBuilder.CreateIndex(
                name: "IX_Balances_EnrollmentId1",
                table: "Balances",
                column: "EnrollmentId1",
                unique: true,
                filter: "[EnrollmentId1] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_Balances_Enrollments_EnrollmentId1",
                table: "Balances",
                column: "EnrollmentId1",
                principalTable: "Enrollments",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Payments_Enrollments_EnrollmentId1",
                table: "Payments",
                column: "EnrollmentId1",
                principalTable: "Enrollments",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Balances_Enrollments_EnrollmentId1",
                table: "Balances");

            migrationBuilder.DropForeignKey(
                name: "FK_Payments_Enrollments_EnrollmentId1",
                table: "Payments");

            migrationBuilder.DropIndex(
                name: "IX_Payments_EnrollmentId1",
                table: "Payments");

            migrationBuilder.DropIndex(
                name: "IX_Balances_EnrollmentId1",
                table: "Balances");

            migrationBuilder.DropColumn(
                name: "EnrollmentId1",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "EnrollmentId1",
                table: "Balances");
        }
    }
}
