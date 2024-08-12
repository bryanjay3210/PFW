using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API.Migrations
{
    public partial class UpdateDriverLogModuleTablesAddStatusColumns : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "StatusDetail",
                table: "DriverLogs",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "StatusId",
                table: "DriverLogs",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "StatusDetail",
                table: "DriverLogDetails",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "StatusId",
                table: "DriverLogDetails",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "StatusDetail",
                table: "DriverLogs");

            migrationBuilder.DropColumn(
                name: "StatusId",
                table: "DriverLogs");

            migrationBuilder.DropColumn(
                name: "StatusDetail",
                table: "DriverLogDetails");

            migrationBuilder.DropColumn(
                name: "StatusId",
                table: "DriverLogDetails");
        }
    }
}
