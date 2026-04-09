using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TrainingCertificationPlatform.Migrations
{
    /// <inheritdoc />
    public partial class FixTracksManyToMany : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CourseId",
                table: "Tracks",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Tracks_CourseId",
                table: "Tracks",
                column: "CourseId");

            migrationBuilder.AddForeignKey(
                name: "FK_Tracks_Courses_CourseId",
                table: "Tracks",
                column: "CourseId",
                principalTable: "Courses",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tracks_Courses_CourseId",
                table: "Tracks");

            migrationBuilder.DropIndex(
                name: "IX_Tracks_CourseId",
                table: "Tracks");

            migrationBuilder.DropColumn(
                name: "CourseId",
                table: "Tracks");
        }
    }
}
