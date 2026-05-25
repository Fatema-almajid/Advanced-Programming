using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TrainingCertificationPlatform.Migrations
{
    /// <inheritdoc />
    public partial class NewData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Courses",
                keyColumn: "Id",
                keyValue: 1,
                column: "Capacity",
                value: 10);

            migrationBuilder.UpdateData(
                table: "Courses",
                keyColumn: "Id",
                keyValue: 2,
                column: "Capacity",
                value: 15);

            migrationBuilder.UpdateData(
                table: "Courses",
                keyColumn: "Id",
                keyValue: 3,
                column: "Capacity",
                value: 12);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "Password",
                value: "$2a$12$Ys7YXxI9M7EqQY60T8aNFe31SwSs8IGXjfAYsFNp55NcGyzL4cIym");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2,
                column: "Password",
                value: "$2a$12$Ys7YXxI9M7EqQY60T8aNFe31SwSs8IGXjfAYsFNp55NcGyzL4cIym");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 3,
                column: "Password",
                value: "$2a$12$Ys7YXxI9M7EqQY60T8aNFe31SwSs8IGXjfAYsFNp55NcGyzL4cIym");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Courses",
                keyColumn: "Id",
                keyValue: 1,
                column: "Capacity",
                value: 2);

            migrationBuilder.UpdateData(
                table: "Courses",
                keyColumn: "Id",
                keyValue: 2,
                column: "Capacity",
                value: 2);

            migrationBuilder.UpdateData(
                table: "Courses",
                keyColumn: "Id",
                keyValue: 3,
                column: "Capacity",
                value: 2);

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
                column: "Password",
                value: "$2a$12$ZPzIhfjkDv3uc/4fEkhAfuAM/hYixvISLMEhyBYk7dxrsGJdw15Rq");
        }
    }
}
