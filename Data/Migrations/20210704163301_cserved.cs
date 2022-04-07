using Microsoft.EntityFrameworkCore.Migrations;

namespace HubnyxQMS.Data.Migrations
{
    public partial class cserved : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Created",
                table: "QMSUserService",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ServedAllTime",
                table: "QMSUserService",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ServedToday",
                table: "QMSUserService",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Created",
                table: "QMSUserService");

            migrationBuilder.DropColumn(
                name: "ServedAllTime",
                table: "QMSUserService");

            migrationBuilder.DropColumn(
                name: "ServedToday",
                table: "QMSUserService");
        }
    }
}
