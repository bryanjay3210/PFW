using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API.Migrations
{
    public partial class UpdatePartsPickingDetailTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PartsPickingNumber",
                table: "PartsPickingDetails",
                newName: "PurchaseOrderNumber");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PurchaseOrderNumber",
                table: "PartsPickingDetails",
                newName: "PartsPickingNumber");
        }
    }
}
