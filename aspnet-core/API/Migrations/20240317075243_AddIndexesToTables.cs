using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API.Migrations
{
    public partial class AddIndexesToTables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IDX_VENDOR",
                table: "Vendors");

            migrationBuilder.DropIndex(
                name: "IDX_PURCHASE_ORDER",
                table: "PurchaseOrders");

            migrationBuilder.DropIndex(
                name: "IDX_PURCHASE_ORDER_DETAIL",
                table: "PurchaseOrderDetails");

            migrationBuilder.DropIndex(
                name: "IDX_ORDER",
                table: "Orders");

            migrationBuilder.DropIndex(
                name: "IDX_ORDERDETAIL",
                table: "OrderDetails");

            migrationBuilder.CreateIndex(
                name: "IDX_VENDOR",
                table: "Vendors",
                columns: new[] { "VendorName", "VendorCode", "ContactName", "PhoneNumber", "Email", "CAPercentage", "CARank", "NVPercentage", "NVRank" });

            migrationBuilder.CreateIndex(
                name: "IDX_PURCHASE_ORDER",
                table: "PurchaseOrders",
                columns: new[] { "VendorId", "VendorCode", "VendorPO", "PFWBNumber", "ReceivedDate" });

            migrationBuilder.CreateIndex(
                name: "IDX_PURCHASE_ORDER_DETAIL",
                table: "PurchaseOrderDetails",
                columns: new[] { "PurchaseOrderId", "OrderNumber", "PartNumber", "VendorPartNumber", "OrderDate", "ReceivedDate" });

            migrationBuilder.CreateIndex(
                name: "IDX_ORDER",
                table: "Orders",
                columns: new[] { "CustomerId", "AccountNumber", "OrderNumber", "InvoiceNumber", "PurchaseOrderNumber", "RGAType", "RGAReason", "WarehouseId" });

            migrationBuilder.CreateIndex(
                name: "IDX_ORDERDETAIL",
                table: "OrderDetails",
                columns: new[] { "OrderId", "SalesOrderNumber", "OriginalInvoiceNumber", "BuyOutOrder", "PartNumber", "Vehicle", "CategoryId", "TrackingNumber", "IsCreditMemoCreated", "RGAInspectedCode", "RGAPartNumber", "RGALocation", "RGAState" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IDX_VENDOR",
                table: "Vendors");

            migrationBuilder.DropIndex(
                name: "IDX_PURCHASE_ORDER",
                table: "PurchaseOrders");

            migrationBuilder.DropIndex(
                name: "IDX_PURCHASE_ORDER_DETAIL",
                table: "PurchaseOrderDetails");

            migrationBuilder.DropIndex(
                name: "IDX_ORDER",
                table: "Orders");

            migrationBuilder.DropIndex(
                name: "IDX_ORDERDETAIL",
                table: "OrderDetails");

            migrationBuilder.CreateIndex(
                name: "IDX_VENDOR",
                table: "Vendors",
                columns: new[] { "VendorName", "VendorCode", "ContactName", "PhoneNumber", "Email" });

            migrationBuilder.CreateIndex(
                name: "IDX_PURCHASE_ORDER",
                table: "PurchaseOrders",
                columns: new[] { "VendorId", "VendorCode", "VendorPO", "PFWBNumber" });

            migrationBuilder.CreateIndex(
                name: "IDX_PURCHASE_ORDER_DETAIL",
                table: "PurchaseOrderDetails",
                columns: new[] { "PurchaseOrderId", "OrderNumber", "PartNumber", "VendorPartNumber", "OrderDate" });

            migrationBuilder.CreateIndex(
                name: "IDX_ORDER",
                table: "Orders",
                columns: new[] { "CustomerId", "AccountNumber", "OrderNumber", "InvoiceNumber", "PurchaseOrderNumber", "WarehouseId" });

            migrationBuilder.CreateIndex(
                name: "IDX_ORDERDETAIL",
                table: "OrderDetails",
                columns: new[] { "OrderId", "SalesOrderNumber", "OriginalInvoiceNumber", "BuyOutOrder", "PartNumber", "Vehicle", "CategoryId" });
        }
    }
}
