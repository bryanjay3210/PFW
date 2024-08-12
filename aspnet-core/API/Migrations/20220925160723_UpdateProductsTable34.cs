using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API.Migrations
{
    public partial class UpdateProductsTable34 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IDX_PRODUCT",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "OEMListPrice",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "OEMNumber",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "PartSize",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "PartsLinkNumber",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "YearModelRangeFrom",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "YearModelRangeTo",
                table: "Products");

            migrationBuilder.CreateIndex(
                name: "IDX_PRODUCT",
                table: "Products",
                columns: new[] { "PartNumber", "PartDescription", "Brand", "PartSizeId", "CategoryId", "SequenceId" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IDX_PRODUCT",
                table: "Products");

            migrationBuilder.AddColumn<decimal>(
                name: "OEMListPrice",
                table: "Products",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OEMNumber",
                table: "Products",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PartSize",
                table: "Products",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PartsLinkNumber",
                table: "Products",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "YearModelRangeFrom",
                table: "Products",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "YearModelRangeTo",
                table: "Products",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IDX_PRODUCT",
                table: "Products",
                columns: new[] { "PartNumber", "PartDescription", "Brand", "YearModelRangeFrom", "YearModelRangeTo" });
        }
    }
}
