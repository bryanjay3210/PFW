using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API.Migrations
{
    public partial class UpdatePurchaseOrderAndWarehouseTrackingTables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PurchaseOrderDetails_Orders_OrderId",
                table: "PurchaseOrderDetails");

            migrationBuilder.DropIndex(
                name: "IX_PurchaseOrderDetails_OrderId",
                table: "PurchaseOrderDetails");

            migrationBuilder.DropColumn(
                name: "POOrderDate",
                table: "PurchaseOrders");

            migrationBuilder.AlterColumn<int>(
                name: "OrderId",
                table: "PurchaseOrderDetails",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "OrderDetailId",
                table: "PurchaseOrderDetails",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseOrderDetails_OrderDetailId",
                table: "PurchaseOrderDetails",
                column: "OrderDetailId");

            migrationBuilder.AddForeignKey(
                name: "FK_PurchaseOrderDetails_OrderDetails_OrderDetailId",
                table: "PurchaseOrderDetails",
                column: "OrderDetailId",
                principalTable: "OrderDetails",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PurchaseOrderDetails_OrderDetails_OrderDetailId",
                table: "PurchaseOrderDetails");

            migrationBuilder.DropIndex(
                name: "IX_PurchaseOrderDetails_OrderDetailId",
                table: "PurchaseOrderDetails");

            migrationBuilder.DropColumn(
                name: "OrderDetailId",
                table: "PurchaseOrderDetails");

            migrationBuilder.AddColumn<DateTime>(
                name: "POOrderDate",
                table: "PurchaseOrders",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AlterColumn<int>(
                name: "OrderId",
                table: "PurchaseOrderDetails",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseOrderDetails_OrderId",
                table: "PurchaseOrderDetails",
                column: "OrderId");

            migrationBuilder.AddForeignKey(
                name: "FK_PurchaseOrderDetails_Orders_OrderId",
                table: "PurchaseOrderDetails",
                column: "OrderId",
                principalTable: "Orders",
                principalColumn: "Id");
        }
    }
}
