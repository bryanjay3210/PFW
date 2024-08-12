using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API.Migrations
{
    public partial class UpdateItemasterlistReferencesTable_AddIsMainPartsLinkAndIsMainOEMColumns : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.AddColumn<bool>(
            //    name: "IsMainOEM",
            //    table: "ItemMasterlistReferences",
            //    type: "bit",
            //    nullable: false,
            //    defaultValue: false);

            //migrationBuilder.AddColumn<bool>(
            //    name: "IsMainPartsLink",
            //    table: "ItemMasterlistReferences",
            //    type: "bit",
            //    nullable: false,
            //    defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsMainOEM",
                table: "ItemMasterlistReferences");

            migrationBuilder.DropColumn(
                name: "IsMainPartsLink",
                table: "ItemMasterlistReferences");
        }
    }
}
