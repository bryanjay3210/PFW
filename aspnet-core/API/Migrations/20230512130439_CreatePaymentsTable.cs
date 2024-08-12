using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API.Migrations
{
    public partial class CreatePaymentsTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Payments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    InvoiceNumber = table.Column<int>(type: "int", nullable: false),
                    OrderId = table.Column<int>(type: "int", nullable: true),
                    CustomerId = table.Column<int>(type: "int", nullable: false),
                    InvoiceDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AccountNumber = table.Column<int>(type: "int", nullable: false),
                    SalesRepresentativeId = table.Column<int>(type: "int", nullable: false),
                    PurchaseOrderNumber = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    PaymentDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    PaymentAmount = table.Column<decimal>(type: "decimal(12,2)", nullable: true),
                    ReferenceNumber = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: true),
                    Oti = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    AppIto = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    LockedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Lockeddate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CurrencyCode = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: true),
                    ExchangeDifference = table.Column<decimal>(type: "decimal(12,2)", nullable: true),
                    BillPayAmount = table.Column<decimal>(type: "decimal(12,2)", nullable: true),
                    InvoiceRate = table.Column<decimal>(type: "decimal(12,2)", nullable: true),
                    PaymentCurrencyCode = table.Column<string>(type: "nvarchar(6)", maxLength: 6, nullable: true),
                    PaymentRate = table.Column<decimal>(type: "decimal(12,2)", nullable: true),
                    Deposit = table.Column<string>(type: "nvarchar(1)", maxLength: 1, nullable: true),
                    PaymentType = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: true),
                    CheckStatus = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    Particular = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Phone = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Payments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Payments_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Payments_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "SyncOrderResult",
                columns: table => new
                {
                    Value = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                });

            migrationBuilder.CreateIndex(
                name: "IDX_PAYMENT",
                table: "Payments",
                columns: new[] { "InvoiceNumber", "CustomerId", "AccountNumber", "PaymentDate", "ReferenceNumber" });

            migrationBuilder.CreateIndex(
                name: "IX_Payments_CustomerId",
                table: "Payments",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_Payments_OrderId",
                table: "Payments",
                column: "OrderId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Payments");

            migrationBuilder.DropTable(
                name: "SyncOrderResult");
        }
    }
}
