using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API.Migrations
{
    public partial class AddRGAFieldsToOrderDetails : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsCreditMemoCreated",
                table: "OrderDetails",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "RGAInspectedCode",
                table: "OrderDetails",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RGALocation",
                table: "OrderDetails",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RGAPartNumber",
                table: "OrderDetails",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsCreditMemoCreated",
                table: "OrderDetails");

            migrationBuilder.DropColumn(
                name: "RGAInspectedCode",
                table: "OrderDetails");

            migrationBuilder.DropColumn(
                name: "RGALocation",
                table: "OrderDetails");

            migrationBuilder.DropColumn(
                name: "RGAPartNumber",
                table: "OrderDetails");
        }
    }
}
