using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API.Migrations
{
    public partial class UpdateWarehouseLocationsTableAddSequience : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IDX_WAREHOUSELOCATION",
                table: "WarehouseLocations");

            migrationBuilder.AddColumn<int>(
                name: "Sequence",
                table: "WarehouseLocations",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IDX_WAREHOUSELOCATION",
                table: "WarehouseLocations",
                columns: new[] { "WarehouseId", "Location", "Zoning", "Height", "Sequence" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IDX_WAREHOUSELOCATION",
                table: "WarehouseLocations");

            migrationBuilder.DropColumn(
                name: "Sequence",
                table: "WarehouseLocations");

            migrationBuilder.CreateIndex(
                name: "IDX_WAREHOUSELOCATION",
                table: "WarehouseLocations",
                columns: new[] { "WarehouseId", "Location", "Zoning", "Height" });
        }
    }
}
