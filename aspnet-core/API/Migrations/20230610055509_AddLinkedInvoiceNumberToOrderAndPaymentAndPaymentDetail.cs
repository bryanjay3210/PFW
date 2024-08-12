using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API.Migrations
{
    public partial class AddLinkedInvoiceNumberToOrderAndPaymentAndPaymentDetail : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "LinkedInvoiceNumber",
                table: "Payments",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LinkedInvoiceNumber",
                table: "PaymentDetails",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LinkedInvoiceNumber",
                table: "Orders",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LinkedInvoiceNumber",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "LinkedInvoiceNumber",
                table: "PaymentDetails");

            migrationBuilder.DropColumn(
                name: "LinkedInvoiceNumber",
                table: "Orders");
        }
    }
}
