using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API.Migrations
{
    public partial class PartsPickingTables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "ShipZone",
                table: "PurchaseOrderDetails",
                type: "nvarchar(5)",
                maxLength: 5,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateTable(
                name: "PartsPickings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PickNumber = table.Column<string>(type: "nvarchar(25)", maxLength: 25, nullable: false),
                    PickStatus = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    PartsPickingDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsPrinted = table.Column<bool>(type: "bit", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PartsPickings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PartsPickingDetails",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PartsPickingId = table.Column<int>(type: "int", nullable: false),
                    OrderDetailId = table.Column<int>(type: "int", nullable: false),
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    OrderId = table.Column<int>(type: "int", nullable: false),
                    OrderNumber = table.Column<int>(type: "int", nullable: false),
                    OrderQuantity = table.Column<int>(type: "int", nullable: false),
                    StockQuantity = table.Column<int>(type: "int", nullable: false),
                    ShipZone = table.Column<string>(type: "nvarchar(5)", maxLength: 5, nullable: false),
                    PartNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Location = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Sequence = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    PartDescription = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    OrderDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DeliveryDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DeliveryRoute = table.Column<int>(type: "int", nullable: false),
                    PODetailStatus = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    DeliveryMethod = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    MainPartsLinkNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    PartsPickingNumber = table.Column<string>(type: "nvarchar(35)", maxLength: 35, nullable: false),
                    CustomerName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PartsPickingDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PartsPickingDetails_OrderDetails_OrderDetailId",
                        column: x => x.OrderDetailId,
                        principalTable: "OrderDetails",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PartsPickingDetails_PartsPickings_PartsPickingId",
                        column: x => x.PartsPickingId,
                        principalTable: "PartsPickings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PartsPickingDetails_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IDX_PARTS_PICKING_DETAIL",
                table: "PartsPickingDetails",
                columns: new[] { "PartsPickingId", "OrderNumber", "PartNumber", "OrderId", "OrderDate" });

            migrationBuilder.CreateIndex(
                name: "IX_PartsPickingDetails_OrderDetailId",
                table: "PartsPickingDetails",
                column: "OrderDetailId");

            migrationBuilder.CreateIndex(
                name: "IX_PartsPickingDetails_ProductId",
                table: "PartsPickingDetails",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IDX_PARTS_PICKING",
                table: "PartsPickings",
                column: "PickNumber");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PartsPickingDetails");

            migrationBuilder.DropTable(
                name: "PartsPickings");

            migrationBuilder.AlterColumn<string>(
                name: "ShipZone",
                table: "PurchaseOrderDetails",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(5)",
                oldMaxLength: 5);
        }
    }
}
