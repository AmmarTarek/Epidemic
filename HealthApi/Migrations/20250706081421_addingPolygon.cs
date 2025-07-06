using Microsoft.EntityFrameworkCore.Migrations;
using NetTopologySuite.Geometries;

#nullable disable

namespace HealthApi.Migrations
{
    /// <inheritdoc />
    public partial class addingPolygon : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Polygon>(
                name: "Geometry",
                table: "RiskAreas",
                type: "geometry",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Geometry",
                table: "RiskAreas");
        }
    }
}
