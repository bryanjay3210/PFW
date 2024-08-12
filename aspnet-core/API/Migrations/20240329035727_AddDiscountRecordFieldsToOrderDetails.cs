using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API.Migrations
{
    public partial class AddDiscountRecordFieldsToOrderDetails : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "DiscountAmount",
                table: "OrderDetails",
                type: "decimal(12,2)",
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(decimal),
                oldType: "decimal(12,2)",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "Discount",
                table: "OrderDetails",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(decimal),
                oldType: "decimal(12,2)",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DiscountRecord",
                table: "OrderDetails",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "DiscountRecordAmount",
                table: "OrderDetails",
                type: "decimal(12,2)",
                nullable: false,
                defaultValue: 0m);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DiscountRecord",
                table: "OrderDetails");

            migrationBuilder.DropColumn(
                name: "DiscountRecordAmount",
                table: "OrderDetails");

            migrationBuilder.AlterColumn<decimal>(
                name: "DiscountAmount",
                table: "OrderDetails",
                type: "decimal(12,2)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(12,2)");

            migrationBuilder.AlterColumn<decimal>(
                name: "Discount",
                table: "OrderDetails",
                type: "decimal(12,2)",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");
        }
    }
}
