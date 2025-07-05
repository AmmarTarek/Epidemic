using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HealthApi.Migrations
{
    /// <inheritdoc />
    public partial class testRecordUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Details",
                table: "TestRecords",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Details",
                table: "TestRecords");
        }
    }
}
