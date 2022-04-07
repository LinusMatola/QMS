using Microsoft.EntityFrameworkCore.Migrations;

namespace HubnyxQMS.Data.Migrations
{
    public partial class memm : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AverageServingTime",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "ServingMember",
                table: "AspNetUsers",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<double>(
                name: "SuccessRateAlltime",
                table: "AspNetUsers",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "SuccessRateToday",
                table: "AspNetUsers",
                type: "float",
                nullable: false,
                defaultValue: 0.0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AverageServingTime",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "ServingMember",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "SuccessRateAlltime",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "SuccessRateToday",
                table: "AspNetUsers");
        }
    }
}
