using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API.Migrations
{
    public partial class UpdateOrderDetailsTableAddPartsLiknOEMsVendorCodes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "OEMs",
                table: "OrderDetails",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PartsLinks",
                table: "OrderDetails",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "VendorCodes",
                table: "OrderDetails",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OEMs",
                table: "OrderDetails");

            migrationBuilder.DropColumn(
                name: "PartsLinks",
                table: "OrderDetails");

            migrationBuilder.DropColumn(
                name: "VendorCodes",
                table: "OrderDetails");
        }
    }
}
