using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API.Migrations
{
    public partial class AddPartsManifestTables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PartsManifests",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DriverId = table.Column<int>(type: "int", nullable: false),
                    DriverName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    PartsManifestNumber = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    PurposeId = table.Column<int>(type: "int", nullable: false),
                    PurposeName = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: false),
                    VendorId = table.Column<int>(type: "int", nullable: true),
                    VendorCode = table.Column<string>(type: "nvarchar(25)", maxLength: 25, nullable: true),
                    VendorName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PartsManifests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PartsManifests_Drivers_DriverId",
                        column: x => x.DriverId,
                        principalTable: "Drivers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PartsManifestDetails",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PartsManifestId = table.Column<int>(type: "int", nullable: false),
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    PartNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    PartDescription = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PartsManifestDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PartsManifestDetails_PartsManifests_PartsManifestId",
                        column: x => x.PartsManifestId,
                        principalTable: "PartsManifests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PartsManifestDetails_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IDX_PARTSMANIFESTDETAIL",
                table: "PartsManifestDetails",
                columns: new[] { "PartsManifestId", "ProductId", "PartNumber" });

            migrationBuilder.CreateIndex(
                name: "IX_PartsManifestDetails_ProductId",
                table: "PartsManifestDetails",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IDX_PARTSMANIFEST",
                table: "PartsManifests",
                columns: new[] { "DriverId", "DriverName", "PartsManifestNumber" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PartsManifestDetails");

            migrationBuilder.DropTable(
                name: "PartsManifests");
        }
    }
}
