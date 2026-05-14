using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TrainingCertificationPlatform.Migrations
{
    /// <inheritdoc />
    public partial class FixPasswords : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "Amount",
                table: "Payments",
                type: "decimal(10,2)",
                precision: 10,
                scale: 2,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<decimal>(
                name: "AmountDue",
                table: "Balances",
                type: "decimal(10,2)",
                precision: 10,
                scale: 2,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "Password",
                value: "$2a$12$ZPzIhfjkDv3uc/4fEkhAfuAM/hYixvISLMEhyBYk7dxrsGJdw15Rq");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2,
                column: "Password",
                value: "$2a$12$ZPzIhfjkDv3uc/4fEkhAfuAM/hYixvISLMEhyBYk7dxrsGJdw15Rq");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "Email", "FirstName", "LastName", "Password" },
                values: new object[] { "dana@mail.com", "Dana", "Albanki", "$2a$12$ZPzIhfjkDv3uc/4fEkhAfuAM/hYixvISLMEhyBYk7dxrsGJdw15Rq" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 4,
                column: "Password",
                value: "$2a$12$ZPzIhfjkDv3uc/4fEkhAfuAM/hYixvISLMEhyBYk7dxrsGJdw15Rq");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 5,
                column: "Password",
                value: "$2a$12$ZPzIhfjkDv3uc/4fEkhAfuAM/hYixvISLMEhyBYk7dxrsGJdw15Rq");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "Amount",
                table: "Payments",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(10,2)",
                oldPrecision: 10,
                oldScale: 2);

            migrationBuilder.AlterColumn<decimal>(
                name: "AmountDue",
                table: "Balances",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(10,2)",
                oldPrecision: 10,
                oldScale: 2);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "Password",
                value: "$2a$11$x4qb8776Dbr0Ltc2M/tM/O5kFl0tzNF1agUUs543BeGCMz/K/vaT.");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2,
                column: "Password",
                value: "$2a$11$wfxwG1i.WiTjstYbx6pE3ONCKTQYKSqSynJQiCuT1YI2CSjL8Jrsq");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "Email", "FirstName", "LastName", "Password" },
                values: new object[] { "mariam@mail.com", "Mariam", "Hassan", "$2a$11$YvIaDgzxV1IRx9uX6cegOenui7ECXHkzuh/SjzbuYr7kut6IMXg.." });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 4,
                column: "Password",
                value: "$2a$11$SCuxvvUXWltUe7Hz3WGQN.m2LwyW4nCGeZbEmC/ioPGfb9rVA9wbm");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 5,
                column: "Password",
                value: "$2a$11$sRMBUwJ.sjwigPf16zqWIeZpuyvn.t4TBM0oSNvpbRr/w6ovF0uTO");
        }
    }
}
