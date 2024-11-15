using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CoalMineApi.Migrations
{
    /// <inheritdoc />
    public partial class Buffersize : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "BufferSize",
                schema: "CoalMine",
                table: "Coverages",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BufferSize",
                schema: "CoalMine",
                table: "Coverages");
        }
    }
}
