using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookProject.Data.Migrations
{
    public partial class MyMigration15 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "FileType",
                table: "DocumentInformation",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FileType",
                table: "DocumentInformation");
        }
    }
}
