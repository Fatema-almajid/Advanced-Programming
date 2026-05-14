using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TrainingCertificationPlatform.Migrations
{
    /// <inheritdoc />
    public partial class FixMoneyTypes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
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

            migrationBuilder.AlterColumn<decimal>(
                name: "Amount",
                table: "Payments",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "float");

            migrationBuilder.AlterColumn<decimal>(
                name: "AmountDue",
                table: "Balances",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.UpdateData(
                table: "Balances",
                keyColumn: "Id",
                keyValue: 1,
                column: "AmountDue",
                value: 50m);

            migrationBuilder.UpdateData(
                table: "Payments",
                keyColumn: "Id",
                keyValue: 1,
                column: "Amount",
                value: 100m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<double>(
                name: "Amount",
                table: "Payments",
                type: "float",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AddColumn<int>(
                name: "EnrollmentId1",
                table: "Payments",
                type: "int",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "AmountDue",
                table: "Balances",
                type: "int",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AddColumn<int>(
                name: "EnrollmentId1",
                table: "Balances",
                type: "int",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Balances",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "AmountDue", "EnrollmentId1" },
                values: new object[] { 50, null });

            migrationBuilder.UpdateData(
                table: "Payments",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Amount", "EnrollmentId1" },
                values: new object[] { 100.0, null });

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
    }
}
