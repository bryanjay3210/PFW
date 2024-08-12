using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API.Migrations
{
    public partial class UpdatePaymentTables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Payments_Orders_OrderId",
                table: "Payments");

            migrationBuilder.DropIndex(
                name: "IDX_PAYMENT",
                table: "Payments");

            migrationBuilder.DropIndex(
                name: "IX_Payments_CustomerId",
                table: "Payments");

            migrationBuilder.DropIndex(
                name: "IX_CustomerCredits_CustomerId",
                table: "CustomerCredits");

            migrationBuilder.DropColumn(
                name: "AppIto",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "InvoiceNumber",
                table: "Payments");

            migrationBuilder.RenameColumn(
                name: "OrderId",
                table: "Payments",
                newName: "CustomerCreditId");

            migrationBuilder.RenameColumn(
                name: "BillPayAmount",
                table: "Payments",
                newName: "TotalAmountDue");

            migrationBuilder.RenameIndex(
                name: "IX_Payments_OrderId",
                table: "Payments",
                newName: "IX_Payments_CustomerCreditId");

            migrationBuilder.AlterColumn<int>(
                name: "SalesRepresentativeId",
                table: "Payments",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<string>(
                name: "AppliesTo",
                table: "Payments",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CheckNumber",
                table: "Payments",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "CustomerCreditAmountUsed",
                table: "Payments",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsReversePayment",
                table: "Payments",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsUseCustomerCredit",
                table: "Payments",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<decimal>(
                name: "PaymentBalance",
                table: "Payments",
                type: "decimal(12,2)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreditReason",
                table: "CustomerCredits",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "CreditType",
                table: "CustomerCredits",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ReferenceId",
                table: "CustomerCredits",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IDX_PAYMENT1",
                table: "Payments",
                columns: new[] { "CustomerId", "AccountNumber", "PaymentDate", "ReferenceNumber" });

            migrationBuilder.CreateIndex(
                name: "IDX_PAYMENT",
                table: "CustomerCredits",
                columns: new[] { "CustomerId", "ReferenceId", "CreditType" });

            migrationBuilder.AddForeignKey(
                name: "FK_Payments_CustomerCredits_CustomerCreditId",
                table: "Payments",
                column: "CustomerCreditId",
                principalTable: "CustomerCredits",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Payments_CustomerCredits_CustomerCreditId",
                table: "Payments");

            migrationBuilder.DropIndex(
                name: "IDX_PAYMENT1",
                table: "Payments");

            migrationBuilder.DropIndex(
                name: "IDX_PAYMENT",
                table: "CustomerCredits");

            migrationBuilder.DropColumn(
                name: "AppliesTo",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "CheckNumber",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "CustomerCreditAmountUsed",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "IsReversePayment",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "IsUseCustomerCredit",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "PaymentBalance",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "CreditReason",
                table: "CustomerCredits");

            migrationBuilder.DropColumn(
                name: "CreditType",
                table: "CustomerCredits");

            migrationBuilder.DropColumn(
                name: "ReferenceId",
                table: "CustomerCredits");

            migrationBuilder.RenameColumn(
                name: "TotalAmountDue",
                table: "Payments",
                newName: "BillPayAmount");

            migrationBuilder.RenameColumn(
                name: "CustomerCreditId",
                table: "Payments",
                newName: "OrderId");

            migrationBuilder.RenameIndex(
                name: "IX_Payments_CustomerCreditId",
                table: "Payments",
                newName: "IX_Payments_OrderId");

            migrationBuilder.AlterColumn<int>(
                name: "SalesRepresentativeId",
                table: "Payments",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AppIto",
                table: "Payments",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "InvoiceNumber",
                table: "Payments",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IDX_PAYMENT",
                table: "Payments",
                columns: new[] { "InvoiceNumber", "CustomerId", "AccountNumber", "PaymentDate", "ReferenceNumber" });

            migrationBuilder.CreateIndex(
                name: "IX_Payments_CustomerId",
                table: "Payments",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerCredits_CustomerId",
                table: "CustomerCredits",
                column: "CustomerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Payments_Orders_OrderId",
                table: "Payments",
                column: "OrderId",
                principalTable: "Orders",
                principalColumn: "Id");
        }
    }
}
