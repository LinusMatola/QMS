using Microsoft.EntityFrameworkCore.Migrations;

namespace HubnyxQMS.Data.Migrations
{
    public partial class userupt : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Created",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NowServing",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RatingAllTime",
                table: "AspNetUsers",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "RatingToday",
                table: "AspNetUsers",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ServedAlltime",
                table: "AspNetUsers",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ServedToday",
                table: "AspNetUsers",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Created",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "NowServing",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "RatingAllTime",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "RatingToday",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "ServedAlltime",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "ServedToday",
                table: "AspNetUsers");
        }
    }
}
