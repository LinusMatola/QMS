using Microsoft.EntityFrameworkCore.Migrations;

namespace HubnyxQMS.Data.Migrations
{
    public partial class ratt : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "RatingAverage",
                table: "ServicesReports",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "RatingAverage",
                table: "SectionReports",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "AverageRatingAlltime",
                table: "AspNetUsers",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "AverageRatingToday",
                table: "AspNetUsers",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RatingAverage",
                table: "ServicesReports");

            migrationBuilder.DropColumn(
                name: "RatingAverage",
                table: "SectionReports");

            migrationBuilder.DropColumn(
                name: "AverageRatingAlltime",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "AverageRatingToday",
                table: "AspNetUsers");
        }
    }
}
