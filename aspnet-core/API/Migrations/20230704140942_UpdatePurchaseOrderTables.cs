using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API.Migrations
{
    public partial class UpdatePurchaseOrderTables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "EntryDate",
                table: "PurchaseOrderDetails",
                newName: "OrderDate");

            migrationBuilder.AddColumn<DateTime>(
                name: "POOrderDate",
                table: "PurchaseOrders",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "POOrderDate",
                table: "PurchaseOrders");

            migrationBuilder.RenameColumn(
                name: "OrderDate",
                table: "PurchaseOrderDetails",
                newName: "EntryDate");
        }
    }
}
