using Microsoft.EntityFrameworkCore.Migrations;
using NetTopologySuite.Geometries;

#nullable disable

namespace HealthApi.Migrations
{
    /// <inheritdoc />
    public partial class addingfutre : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "QRCodeId",
                table: "AnonymousCheckIns");

            migrationBuilder.AlterColumn<Point>(
                name: "Location",
                table: "AnonymousCheckIns",
                type: "geography",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Location",
                table: "AnonymousCheckIns",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(Point),
                oldType: "geography");

            migrationBuilder.AddColumn<string>(
                name: "QRCodeId",
                table: "AnonymousCheckIns",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
