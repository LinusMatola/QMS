using Microsoft.EntityFrameworkCore.Migrations;

namespace HubnyxQMS.Data.Migrations
{
    public partial class todayticketss : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SectionId",
                table: "TodaysTickets",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_TodaysTickets_SectionId",
                table: "TodaysTickets",
                column: "SectionId");

            migrationBuilder.AddForeignKey(
                name: "FK_TodaysTickets_Sections_SectionId",
                table: "TodaysTickets",
                column: "SectionId",
                principalTable: "Sections",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TodaysTickets_Sections_SectionId",
                table: "TodaysTickets");

            migrationBuilder.DropIndex(
                name: "IX_TodaysTickets_SectionId",
                table: "TodaysTickets");

            migrationBuilder.DropColumn(
                name: "SectionId",
                table: "TodaysTickets");
        }
    }
}
