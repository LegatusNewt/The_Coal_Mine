using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CoalMineApi.Migrations
{
    /// <inheritdoc />
    public partial class methanecasing : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Ch4",
                schema: "CoalMine",
                table: "Emissions",
                newName: "CH4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CH4",
                schema: "CoalMine",
                table: "Emissions",
                newName: "Ch4");
        }
    }
}
