using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SudokuMVC.Migrations
{
    /// <inheritdoc />
    public partial class UpdateLeaderboardSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Score",
                table: "LeaderboardEntries",
                newName: "StopwatchValue");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "StopwatchValue",
                table: "LeaderboardEntries",
                newName: "Score");
        }
    }
}
