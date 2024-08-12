using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API.Migrations
{
    public partial class UpdateStockPartsLocationsTableAddIndex : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IDX_STOCKPARTSLOCATION",
                table: "StockPartsLocations",
                columns: new[] { "PartNumber", "Location" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IDX_STOCKPARTSLOCATION",
                table: "StockPartsLocations");
        }
    }
}
