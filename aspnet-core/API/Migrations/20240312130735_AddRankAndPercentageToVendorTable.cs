using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API.Migrations
{
    public partial class AddRankAndPercentageToVendorTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CAPercentage",
                table: "Vendors",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "CARank",
                table: "Vendors",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "NVPercentage",
                table: "Vendors",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "NVRank",
                table: "Vendors",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CAPercentage",
                table: "Vendors");

            migrationBuilder.DropColumn(
                name: "CARank",
                table: "Vendors");

            migrationBuilder.DropColumn(
                name: "NVPercentage",
                table: "Vendors");

            migrationBuilder.DropColumn(
                name: "NVRank",
                table: "Vendors");
        }
    }
}
