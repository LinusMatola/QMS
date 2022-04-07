using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace HubnyxQMS.Data.Migrations
{
    public partial class seccc : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AverageServingTime",
                table: "Reports",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "AverageWaitingTime",
                table: "Reports",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Created",
                table: "Reports",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Date",
                table: "Reports",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "EscalatedCases",
                table: "Reports",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "EscalatedCasesAverage",
                table: "Reports",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "EscalatedCasesSolved",
                table: "Reports",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Rating",
                table: "Reports",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TotalMembers",
                table: "Reports",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TotalServed",
                table: "Reports",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TotalServingTime",
                table: "Reports",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TotalWaitingTime",
                table: "Reports",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "SectionReports",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Report = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Created = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TotalMembers = table.Column<int>(type: "int", nullable: false),
                    TotalServed = table.Column<int>(type: "int", nullable: false),
                    AverageWaitingTime = table.Column<int>(type: "int", nullable: false),
                    TotalWaitingTime = table.Column<int>(type: "int", nullable: false),
                    AverageServingTime = table.Column<int>(type: "int", nullable: false),
                    TotalServingTime = table.Column<int>(type: "int", nullable: false),
                    Rating = table.Column<int>(type: "int", nullable: false),
                    EscalatedCases = table.Column<int>(type: "int", nullable: false),
                    EscalatedCasesSolved = table.Column<int>(type: "int", nullable: false),
                    EscalatedCasesAverage = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SectionReports", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ServicesReports",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Report = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Created = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TotalMembers = table.Column<int>(type: "int", nullable: false),
                    TotalServed = table.Column<int>(type: "int", nullable: false),
                    AverageWaitingTime = table.Column<int>(type: "int", nullable: false),
                    TotalWaitingTime = table.Column<int>(type: "int", nullable: false),
                    AverageServingTime = table.Column<int>(type: "int", nullable: false),
                    TotalServingTime = table.Column<int>(type: "int", nullable: false),
                    Rating = table.Column<int>(type: "int", nullable: false),
                    EscalatedCases = table.Column<int>(type: "int", nullable: false),
                    EscalatedCasesSolved = table.Column<int>(type: "int", nullable: false),
                    EscalatedCasesAverage = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServicesReports", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SectionReports");

            migrationBuilder.DropTable(
                name: "ServicesReports");

            migrationBuilder.DropColumn(
                name: "AverageServingTime",
                table: "Reports");

            migrationBuilder.DropColumn(
                name: "AverageWaitingTime",
                table: "Reports");

            migrationBuilder.DropColumn(
                name: "Created",
                table: "Reports");

            migrationBuilder.DropColumn(
                name: "Date",
                table: "Reports");

            migrationBuilder.DropColumn(
                name: "EscalatedCases",
                table: "Reports");

            migrationBuilder.DropColumn(
                name: "EscalatedCasesAverage",
                table: "Reports");

            migrationBuilder.DropColumn(
                name: "EscalatedCasesSolved",
                table: "Reports");

            migrationBuilder.DropColumn(
                name: "Rating",
                table: "Reports");

            migrationBuilder.DropColumn(
                name: "TotalMembers",
                table: "Reports");

            migrationBuilder.DropColumn(
                name: "TotalServed",
                table: "Reports");

            migrationBuilder.DropColumn(
                name: "TotalServingTime",
                table: "Reports");

            migrationBuilder.DropColumn(
                name: "TotalWaitingTime",
                table: "Reports");
        }
    }
}
