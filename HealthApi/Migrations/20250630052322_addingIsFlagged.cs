using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HealthApi.Migrations
{
    /// <inheritdoc />
    public partial class addingIsFlagged : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsFlagged",
                table: "Users",
                type: "bit",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsFlagged",
                table: "Users");
        }
    }
}
