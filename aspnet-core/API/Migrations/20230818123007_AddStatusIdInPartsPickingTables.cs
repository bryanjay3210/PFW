using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API.Migrations
{
    public partial class AddStatusIdInPartsPickingTables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IDX_PARTS_PICKING",
                table: "PartsPickings");

            migrationBuilder.DropIndex(
                name: "IDX_PARTS_PICKING_DETAIL",
                table: "PartsPickingDetails");

            migrationBuilder.RenameColumn(
                name: "PODetailStatus",
                table: "PartsPickingDetails",
                newName: "PPDetailStatus");

            migrationBuilder.AddColumn<int>(
                name: "StatusId",
                table: "PartsPickings",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "StatusId",
                table: "PartsPickingDetails",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IDX_PARTS_PICKING",
                table: "PartsPickings",
                columns: new[] { "PickNumber", "StatusId", "PickStatus" });

            migrationBuilder.CreateIndex(
                name: "IDX_PARTS_PICKING_DETAIL",
                table: "PartsPickingDetails",
                columns: new[] { "PartsPickingId", "OrderNumber", "PartNumber", "OrderId", "OrderDate", "StatusId", "PPDetailStatus" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IDX_PARTS_PICKING",
                table: "PartsPickings");

            migrationBuilder.DropIndex(
                name: "IDX_PARTS_PICKING_DETAIL",
                table: "PartsPickingDetails");

            migrationBuilder.DropColumn(
                name: "StatusId",
                table: "PartsPickings");

            migrationBuilder.DropColumn(
                name: "StatusId",
                table: "PartsPickingDetails");

            migrationBuilder.RenameColumn(
                name: "PPDetailStatus",
                table: "PartsPickingDetails",
                newName: "PODetailStatus");

            migrationBuilder.CreateIndex(
                name: "IDX_PARTS_PICKING",
                table: "PartsPickings",
                column: "PickNumber");

            migrationBuilder.CreateIndex(
                name: "IDX_PARTS_PICKING_DETAIL",
                table: "PartsPickingDetails",
                columns: new[] { "PartsPickingId", "OrderNumber", "PartNumber", "OrderId", "OrderDate" });
        }
    }
}
