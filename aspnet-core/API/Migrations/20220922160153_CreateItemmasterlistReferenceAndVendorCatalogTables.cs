using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API.Migrations
{
    public partial class CreateItemmasterlistReferenceAndVendorCatalogTables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IDX_PRODUCT",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "OEM",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "OEMId",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "PartslinkId",
                table: "Products");

            migrationBuilder.RenameColumn(
                name: "PartslinkNumber",
                table: "Products",
                newName: "PartsLinkNumber");

            migrationBuilder.AlterColumn<string>(
                name: "PartsLinkNumber",
                table: "Products",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "PartNumber",
                table: "Products",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ImageUrl",
                table: "Products",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Brand",
                table: "Products",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OEMNumber",
                table: "Products",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ItemMasterlistReferences",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PartNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    PartsLinkNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    OEMNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItemMasterlistReferences", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "VendorCatalogs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    VendorCode = table.Column<string>(type: "nvarchar(25)", maxLength: 25, nullable: false),
                    VendorPartNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    PartsLinkNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    OnHand = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VendorCatalogs", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IDX_PRODUCT",
                table: "Products",
                columns: new[] { "PartNumber", "PartDescription", "Brand", "YearModelRangeFrom", "YearModelRangeTo" });

            migrationBuilder.CreateIndex(
                name: "IDX_ITEMMASTERLISTREFERENCE",
                table: "ItemMasterlistReferences",
                columns: new[] { "PartNumber", "PartsLinkNumber", "OEMNumber" });

            migrationBuilder.CreateIndex(
                name: "IDX_ITEMMASTERLISTREFERENCE1",
                table: "VendorCatalogs",
                columns: new[] { "VendorCode", "VendorPartNumber", "PartsLinkNumber" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ItemMasterlistReferences");

            migrationBuilder.DropTable(
                name: "VendorCatalogs");

            migrationBuilder.DropIndex(
                name: "IDX_PRODUCT",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "OEMNumber",
                table: "Products");

            migrationBuilder.RenameColumn(
                name: "PartsLinkNumber",
                table: "Products",
                newName: "PartslinkNumber");

            migrationBuilder.AlterColumn<string>(
                name: "PartslinkNumber",
                table: "Products",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "PartNumber",
                table: "Products",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20);

            migrationBuilder.AlterColumn<string>(
                name: "ImageUrl",
                table: "Products",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Brand",
                table: "Products",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OEM",
                table: "Products",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "OEMId",
                table: "Products",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PartslinkId",
                table: "Products",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IDX_PRODUCT",
                table: "Products",
                columns: new[] { "PartNumber", "PartDescription", "YearModelRangeFrom", "YearModelRangeTo" });
        }
    }
}
