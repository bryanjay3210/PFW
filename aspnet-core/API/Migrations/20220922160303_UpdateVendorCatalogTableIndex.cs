using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API.Migrations
{
    public partial class UpdateVendorCatalogTableIndex : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameIndex(
                name: "IDX_ITEMMASTERLISTREFERENCE1",
                table: "VendorCatalogs",
                newName: "IDX_VENDORCATALOG");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameIndex(
                name: "IDX_VENDORCATALOG",
                table: "VendorCatalogs",
                newName: "IDX_ITEMMASTERLISTREFERENCE1");
        }
    }
}
