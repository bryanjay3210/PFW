using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API.Migrations
{
    public partial class UpdateDriverLogTablesColumnShipAddressName : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ShipContactName",
                table: "DriverLogDetails");

            migrationBuilder.AddColumn<string>(
                name: "ShipAddressName",
                table: "DriverLogDetails",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ShipAddressName",
                table: "DriverLogDetails");

            migrationBuilder.AddColumn<string>(
                name: "ShipContactName",
                table: "DriverLogDetails",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");
        }
    }
}
