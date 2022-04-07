using Microsoft.EntityFrameworkCore.Migrations;

namespace HubnyxQMS.Data.Migrations
{
    public partial class withdrawall : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ServiceName",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ServiceName",
                table: "AspNetUsers");
        }
    }
}
