using Microsoft.EntityFrameworkCore.Migrations;

namespace HubnyxQMS.Data.Migrations
{
    public partial class screenupt : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Screens_Services_ServiceId",
                table: "Screens");

            migrationBuilder.DropIndex(
                name: "IX_Screens_ServiceId",
                table: "Screens");

            migrationBuilder.DropColumn(
                name: "ServiceId",
                table: "Screens");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ServiceId",
                table: "Screens",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Screens_ServiceId",
                table: "Screens",
                column: "ServiceId");

            migrationBuilder.AddForeignKey(
                name: "FK_Screens_Services_ServiceId",
                table: "Screens",
                column: "ServiceId",
                principalTable: "Services",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
