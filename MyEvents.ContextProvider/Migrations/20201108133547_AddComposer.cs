using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MyEvents.ContextProvider.Migrations
{
    public partial class AddComposer : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Composer",
                table: "Events",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Composers",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Composers", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Composers");

            migrationBuilder.DropColumn(
                name: "Composer",
                table: "Events");
        }
    }
}
