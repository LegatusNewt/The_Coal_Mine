using Microsoft.EntityFrameworkCore.Migrations;
using NetTopologySuite.Geometries;

#nullable disable

namespace CoalMineApi.Migrations
{
    /// <inheritdoc />
    public partial class Autoincrementkey : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Polygon",
                schema: "CoalMine",
                table: "Coverages");

            migrationBuilder.AlterColumn<Point>(
                name: "Point",
                schema: "CoalMine",
                table: "Emissions",
                type: "geometry",
                nullable: false,
                oldClrType: typeof(Point),
                oldType: "geometry",
                oldNullable: true);

            migrationBuilder.AddColumn<Polygon>(
                name: "Geometry",
                schema: "CoalMine",
                table: "Coverages",
                type: "geometry",
                nullable: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Geometry",
                schema: "CoalMine",
                table: "Coverages");

            migrationBuilder.AlterColumn<Point>(
                name: "Point",
                schema: "CoalMine",
                table: "Emissions",
                type: "geometry",
                nullable: true,
                oldClrType: typeof(Point),
                oldType: "geometry");

            migrationBuilder.AddColumn<Polygon>(
                name: "Polygon",
                schema: "CoalMine",
                table: "Coverages",
                type: "geometry",
                nullable: true);
        }
    }
}
