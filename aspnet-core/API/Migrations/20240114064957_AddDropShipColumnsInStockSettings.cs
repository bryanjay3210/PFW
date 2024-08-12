using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API.Migrations
{
    public partial class AddDropShipColumnsInStockSettings : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.AddColumn<bool>(
            //    name: "IsDropShipCAProduct",
            //    table: "StockSettings",
            //    type: "bit",
            //    nullable: false,
            //    defaultValue: false);

            //migrationBuilder.AddColumn<bool>(
            //    name: "IsDropShipNVProduct",
            //    table: "StockSettings",
            //    type: "bit",
            //    nullable: false,
            //    defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.DropColumn(
            //    name: "IsDropShipCAProduct",
            //    table: "StockSettings");

            //migrationBuilder.DropColumn(
            //    name: "IsDropShipNVProduct",
            //    table: "StockSettings");
        }
    }
}
