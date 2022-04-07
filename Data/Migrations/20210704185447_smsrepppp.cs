using Microsoft.EntityFrameworkCore.Migrations;

namespace HubnyxQMS.Data.Migrations
{
    public partial class smsrepppp : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Report",
                table: "AspNetUsers",
                newName: "FullReport");

            migrationBuilder.AddColumn<string>(
                name: "DailyReport",
                table: "Services",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FullReport",
                table: "Services",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DailyReport",
                table: "Sections",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FullReport",
                table: "Sections",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DailyReport",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DailyReport",
                table: "Services");

            migrationBuilder.DropColumn(
                name: "FullReport",
                table: "Services");

            migrationBuilder.DropColumn(
                name: "DailyReport",
                table: "Sections");

            migrationBuilder.DropColumn(
                name: "FullReport",
                table: "Sections");

            migrationBuilder.DropColumn(
                name: "DailyReport",
                table: "AspNetUsers");

            migrationBuilder.RenameColumn(
                name: "FullReport",
                table: "AspNetUsers",
                newName: "Report");
        }
    }
}
