using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TrainingCertificationPlatform.Migrations
{
    /// <inheritdoc />
    public partial class FixTracksManyToMany3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
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

            migrationBuilder.CreateTable(
                name: "CourseTrack",
                columns: table => new
                {
                    CoursesId = table.Column<int>(type: "int", nullable: false),
                    TracksId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CourseTrack", x => new { x.CoursesId, x.TracksId });
                    table.ForeignKey(
                        name: "FK_CourseTrack_Courses_CoursesId",
                        column: x => x.CoursesId,
                        principalTable: "Courses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CourseTrack_Tracks_TracksId",
                        column: x => x.TracksId,
                        principalTable: "Tracks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CourseTrack_TracksId",
                table: "CourseTrack",
                column: "TracksId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CourseTrack");

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
    }
}
