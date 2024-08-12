using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API.Migrations
{
    public partial class CreatePartsCatalogsTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PartsCatalogs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PartNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    ProductId = table.Column<int>(type: "int", nullable: true),
                    Make = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Model = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    YearFrom = table.Column<int>(type: "int", nullable: false),
                    YearTo = table.Column<int>(type: "int", nullable: false),
                    Cylinder = table.Column<int>(type: "int", nullable: false),
                    Liter = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    GroupHead = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    SubGroup = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    SubModel = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    Position = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Brand = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PartsCatalogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PartsCatalogs_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IDX_PARTS_CATALOG",
                table: "PartsCatalogs",
                columns: new[] { "PartNumber", "ProductId", "Make", "Model", "YearFrom", "YearTo", "Cylinder", "Liter", "Brand" });

            migrationBuilder.CreateIndex(
                name: "IX_PartsCatalogs_ProductId",
                table: "PartsCatalogs",
                column: "ProductId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PartsCatalogs");
        }
    }
}
