using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API.Migrations
{
    public partial class UpdateProductsTable3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IDX_PRODUCT",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "Code",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "Partslink",
                table: "Products");

            migrationBuilder.RenameColumn(
                name: "Status",
                table: "Products",
                newName: "PartslinkNumber");

            migrationBuilder.RenameColumn(
                name: "PrivelLevel8",
                table: "Products",
                newName: "PriceLevel8");

            migrationBuilder.RenameColumn(
                name: "PrivelLevel7",
                table: "Products",
                newName: "PriceLevel7");

            migrationBuilder.RenameColumn(
                name: "PrivelLevel6",
                table: "Products",
                newName: "PriceLevel6");

            migrationBuilder.RenameColumn(
                name: "PrivelLevel5",
                table: "Products",
                newName: "PriceLevel5");

            migrationBuilder.RenameColumn(
                name: "PrivelLevel4",
                table: "Products",
                newName: "PriceLevel4");

            migrationBuilder.RenameColumn(
                name: "PrivelLevel3",
                table: "Products",
                newName: "PriceLevel3");

            migrationBuilder.RenameColumn(
                name: "PrivelLevel2",
                table: "Products",
                newName: "PriceLevel2");

            migrationBuilder.RenameColumn(
                name: "PrivelLevel1",
                table: "Products",
                newName: "PriceLevel1");

            migrationBuilder.RenameColumn(
                name: "AvailabilityId",
                table: "Products",
                newName: "YearModelRangeTo");

            migrationBuilder.AlterColumn<string>(
                name: "PartNumber",
                table: "Products",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "PartDescription",
                table: "Products",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<byte[]>(
                name: "Image",
                table: "Products",
                type: "varbinary(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "OEMId",
                table: "Products",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PartSizeId",
                table: "Products",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PartslinkId",
                table: "Products",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "PreviousCost",
                table: "Products",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "StatusId",
                table: "Products",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "YearModelRangeFrom",
                table: "Products",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IDX_PRODUCT",
                table: "Products",
                columns: new[] { "PartNumber", "PartDescription", "YearModelRangeFrom", "YearModelRangeTo" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IDX_PRODUCT",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "Image",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "OEMId",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "PartSizeId",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "PartslinkId",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "PreviousCost",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "StatusId",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "YearModelRangeFrom",
                table: "Products");

            migrationBuilder.RenameColumn(
                name: "YearModelRangeTo",
                table: "Products",
                newName: "AvailabilityId");

            migrationBuilder.RenameColumn(
                name: "PriceLevel8",
                table: "Products",
                newName: "PrivelLevel8");

            migrationBuilder.RenameColumn(
                name: "PriceLevel7",
                table: "Products",
                newName: "PrivelLevel7");

            migrationBuilder.RenameColumn(
                name: "PriceLevel6",
                table: "Products",
                newName: "PrivelLevel6");

            migrationBuilder.RenameColumn(
                name: "PriceLevel5",
                table: "Products",
                newName: "PrivelLevel5");

            migrationBuilder.RenameColumn(
                name: "PriceLevel4",
                table: "Products",
                newName: "PrivelLevel4");

            migrationBuilder.RenameColumn(
                name: "PriceLevel3",
                table: "Products",
                newName: "PrivelLevel3");

            migrationBuilder.RenameColumn(
                name: "PriceLevel2",
                table: "Products",
                newName: "PrivelLevel2");

            migrationBuilder.RenameColumn(
                name: "PriceLevel1",
                table: "Products",
                newName: "PrivelLevel1");

            migrationBuilder.RenameColumn(
                name: "PartslinkNumber",
                table: "Products",
                newName: "Status");

            migrationBuilder.AlterColumn<int>(
                name: "PartNumber",
                table: "Products",
                type: "int",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "PartDescription",
                table: "Products",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Code",
                table: "Products",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Products",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Products",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Partslink",
                table: "Products",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IDX_PRODUCT",
                table: "Products",
                columns: new[] { "Name", "Code", "Description" });
        }
    }
}
