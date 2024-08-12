using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API.Migrations
{
    public partial class updatePaymentsDetailsAddOrderNumber : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IDX_PAYMENT_DETAIL",
                table: "PaymentDetails");

            migrationBuilder.AddColumn<int>(
                name: "OrderNumber",
                table: "PaymentDetails",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IDX_PAYMENT_DETAIL",
                table: "PaymentDetails",
                columns: new[] { "PaymentId", "OrderNumber", "InvoiceNumber" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IDX_PAYMENT_DETAIL",
                table: "PaymentDetails");

            migrationBuilder.DropColumn(
                name: "OrderNumber",
                table: "PaymentDetails");

            migrationBuilder.CreateIndex(
                name: "IDX_PAYMENT_DETAIL",
                table: "PaymentDetails",
                column: "PaymentId");
        }
    }
}
