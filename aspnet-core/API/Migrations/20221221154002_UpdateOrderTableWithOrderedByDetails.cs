using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API.Migrations
{
    public partial class UpdateOrderTableWithOrderedByDetails : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DeliveryDate",
                table: "Orders",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DeliveryRoute",
                table: "Orders",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "OrderedBy",
                table: "Orders",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "OrderedByEmail",
                table: "Orders",
                type: "nvarchar(60)",
                maxLength: 60,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OrderedByNotes",
                table: "Orders",
                type: "nvarchar(2000)",
                maxLength: 2000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OrderedByPhoneNumber",
                table: "Orders",
                type: "nvarchar(15)",
                maxLength: 15,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DeliveryDate",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "DeliveryRoute",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "OrderedBy",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "OrderedByEmail",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "OrderedByNotes",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "OrderedByPhoneNumber",
                table: "Orders");
        }
    }
}
