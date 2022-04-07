using Microsoft.EntityFrameworkCore.Migrations;

namespace HubnyxQMS.Data.Migrations
{
    public partial class canviewdb : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "CanViewDashboard",
                table: "AspNetUsers",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CanViewDashboard",
                table: "AspNetUsers");
        }
    }
}
