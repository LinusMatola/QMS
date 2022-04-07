using Microsoft.EntityFrameworkCore.Migrations;

namespace HubnyxQMS.Data.Migrations
{
    public partial class seccccc : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ServiceId",
                table: "ServicesReports",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SectionId",
                table: "SectionReports",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ServicesReports_ServiceId",
                table: "ServicesReports",
                column: "ServiceId");

            migrationBuilder.CreateIndex(
                name: "IX_SectionReports_SectionId",
                table: "SectionReports",
                column: "SectionId");

            migrationBuilder.AddForeignKey(
                name: "FK_SectionReports_Sections_SectionId",
                table: "SectionReports",
                column: "SectionId",
                principalTable: "Sections",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ServicesReports_Services_ServiceId",
                table: "ServicesReports",
                column: "ServiceId",
                principalTable: "Services",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SectionReports_Sections_SectionId",
                table: "SectionReports");

            migrationBuilder.DropForeignKey(
                name: "FK_ServicesReports_Services_ServiceId",
                table: "ServicesReports");

            migrationBuilder.DropIndex(
                name: "IX_ServicesReports_ServiceId",
                table: "ServicesReports");

            migrationBuilder.DropIndex(
                name: "IX_SectionReports_SectionId",
                table: "SectionReports");

            migrationBuilder.DropColumn(
                name: "ServiceId",
                table: "ServicesReports");

            migrationBuilder.DropColumn(
                name: "SectionId",
                table: "SectionReports");
        }
    }
}
