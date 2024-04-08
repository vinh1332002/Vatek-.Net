using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookProject.Data.Migrations
{
    public partial class MyMigration10 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "BookmarkPageId",
                table: "DocumentPage",
                type: "int",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BookmarkPageId",
                table: "DocumentPage");
        }
    }
}
