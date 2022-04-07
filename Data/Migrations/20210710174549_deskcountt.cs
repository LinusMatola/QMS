using Microsoft.EntityFrameworkCore.Migrations;

namespace HubnyxQMS.Data.Migrations
{
    public partial class deskcountt : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DeskCounterId",
                table: "Screens",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Screens_DeskCounterId",
                table: "Screens",
                column: "DeskCounterId");

            migrationBuilder.AddForeignKey(
                name: "FK_Screens_DeskCounters_DeskCounterId",
                table: "Screens",
                column: "DeskCounterId",
                principalTable: "DeskCounters",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Screens_DeskCounters_DeskCounterId",
                table: "Screens");

            migrationBuilder.DropIndex(
                name: "IX_Screens_DeskCounterId",
                table: "Screens");

            migrationBuilder.DropColumn(
                name: "DeskCounterId",
                table: "Screens");
        }
    }
}
