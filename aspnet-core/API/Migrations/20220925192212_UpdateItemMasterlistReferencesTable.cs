using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API.Migrations
{
    public partial class UpdateItemMasterlistReferencesTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IDX_ITEMMASTERLISTREFERENCE",
                table: "ItemMasterlistReferences");

            migrationBuilder.AddColumn<int>(
                name: "ProductId",
                table: "ItemMasterlistReferences",
                type: "int",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.CreateIndex(
                name: "IDX_ITEMMASTERLISTREFERENCE",
                table: "ItemMasterlistReferences",
                columns: new[] { "ProductId", "PartNumber", "PartsLinkNumber", "OEMNumber" });

            migrationBuilder.AddForeignKey(
                name: "FK_ItemMasterlistReferences_Products_ProductId",
                table: "ItemMasterlistReferences",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ItemMasterlistReferences_Products_ProductId",
                table: "ItemMasterlistReferences");

            migrationBuilder.DropIndex(
                name: "IDX_ITEMMASTERLISTREFERENCE",
                table: "ItemMasterlistReferences");

            migrationBuilder.DropColumn(
                name: "ProductId",
                table: "ItemMasterlistReferences");

            migrationBuilder.CreateIndex(
                name: "IDX_ITEMMASTERLISTREFERENCE",
                table: "ItemMasterlistReferences",
                columns: new[] { "PartNumber", "PartsLinkNumber", "OEMNumber" });
        }
    }
}
