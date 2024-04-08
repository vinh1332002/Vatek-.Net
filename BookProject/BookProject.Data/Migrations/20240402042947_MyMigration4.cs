using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookProject.Data.Migrations
{
    public partial class MyMigration4 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DocumentPage_DocumentInformation_DocumentId",
                table: "DocumentPage");

            migrationBuilder.DropIndex(
                name: "IX_DocumentPage_DocumentId",
                table: "DocumentPage");

            migrationBuilder.DropColumn(
                name: "DocumentId",
                table: "DocumentPage");

            migrationBuilder.AddColumn<int>(
                name: "DocumentInformationId",
                table: "DocumentPage",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_DocumentPage_DocumentInformationId",
                table: "DocumentPage",
                column: "DocumentInformationId");

            migrationBuilder.AddForeignKey(
                name: "FK_DocumentPage_DocumentInformation_DocumentInformationId",
                table: "DocumentPage",
                column: "DocumentInformationId",
                principalTable: "DocumentInformation",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DocumentPage_DocumentInformation_DocumentInformationId",
                table: "DocumentPage");

            migrationBuilder.DropIndex(
                name: "IX_DocumentPage_DocumentInformationId",
                table: "DocumentPage");

            migrationBuilder.DropColumn(
                name: "DocumentInformationId",
                table: "DocumentPage");

            migrationBuilder.AddColumn<int>(
                name: "DocumentId",
                table: "DocumentPage",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_DocumentPage_DocumentId",
                table: "DocumentPage",
                column: "DocumentId");

            migrationBuilder.AddForeignKey(
                name: "FK_DocumentPage_DocumentInformation_DocumentId",
                table: "DocumentPage",
                column: "DocumentId",
                principalTable: "DocumentInformation",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
