using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API.Migrations
{
    public partial class UpdateOrdersAndOrderDetailsRestockingFields : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "RestockingAmount",
                table: "Orders",
                type: "decimal(12,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "RestockingFee",
                table: "Orders",
                type: "decimal(12,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "RestockingAmount",
                table: "OrderDetails",
                type: "decimal(12,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "RestockingFee",
                table: "OrderDetails",
                type: "decimal(12,2)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RestockingAmount",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "RestockingFee",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "RestockingAmount",
                table: "OrderDetails");

            migrationBuilder.DropColumn(
                name: "RestockingFee",
                table: "OrderDetails");
        }
    }
}
