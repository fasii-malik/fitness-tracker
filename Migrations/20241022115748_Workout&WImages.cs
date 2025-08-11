using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FitnessTracker.Migrations
{
    /// <inheritdoc />
    public partial class WorkoutWImages : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Workouts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "100, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Reps = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    WorkoutCategoryId = table.Column<int>(type: "int", nullable: false),
                    HomeImgUrl = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Workouts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Workouts_WorkoutCategories_WorkoutCategoryId",
                        column: x => x.WorkoutCategoryId,
                        principalTable: "WorkoutCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WImages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "10, 1"),
                    WorkoutId = table.Column<int>(type: "int", nullable: false),
                    WorkoutImgUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    WorkoutName = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WImages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WImages_Workouts_WorkoutId",
                        column: x => x.WorkoutId,
                        principalTable: "Workouts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_WImages_WorkoutId",
                table: "WImages",
                column: "WorkoutId");

            migrationBuilder.CreateIndex(
                name: "IX_Workouts_WorkoutCategoryId",
                table: "Workouts",
                column: "WorkoutCategoryId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "WImages");

            migrationBuilder.DropTable(
                name: "Workouts");
        }
    }
}
