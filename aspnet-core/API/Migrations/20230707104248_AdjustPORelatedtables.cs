using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API.Migrations
{
    public partial class AdjustPORelatedtables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CategoryGroupDescription",
                table: "PurchaseOrderDetails",
                newName: "Sequence");

            migrationBuilder.AddColumn<string>(
                name: "DeliveryMethod",
                table: "PurchaseOrders",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "MainPartsLinkNumber",
                table: "PurchaseOrderDetails",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DeliveryMethod",
                table: "PurchaseOrders");

            migrationBuilder.DropColumn(
                name: "MainPartsLinkNumber",
                table: "PurchaseOrderDetails");

            migrationBuilder.RenameColumn(
                name: "Sequence",
                table: "PurchaseOrderDetails",
                newName: "CategoryGroupDescription");
        }
    }
}
