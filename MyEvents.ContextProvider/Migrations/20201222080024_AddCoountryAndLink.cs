using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MyEvents.ContextProvider.Migrations
{
    public partial class AddCoountryAndLink : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Country",
                table: "Venues",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Link",
                table: "Events",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PerformanceCountry",
                table: "Events",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Countries",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Countries", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Countries");

            migrationBuilder.DropColumn(
                name: "Country",
                table: "Venues");

            migrationBuilder.DropColumn(
                name: "Link",
                table: "Events");

            migrationBuilder.DropColumn(
                name: "PerformanceCountry",
                table: "Events");
        }
    }
}
