using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HealthApi.Migrations
{
    /// <inheritdoc />
    public partial class VaccineRecordUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "StatusMessage",
                table: "VaccineRecords",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "StatusMessage",
                table: "VaccineRecords");
        }
    }
}
