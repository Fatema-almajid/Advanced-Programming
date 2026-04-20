using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TrainingCertificationPlatform.Migrations
{
    /// <inheritdoc />
    public partial class AddInstructorExpertise : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_InstructorExpertises_CourseId",
                table: "InstructorExpertises",
                column: "CourseId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "InstructorExpertises");
        }
    }
}
