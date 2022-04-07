using Microsoft.EntityFrameworkCore.Migrations;

namespace HubnyxQMS.Data.Migrations
{
    public partial class todaytickets : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TodaysTickets",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    TicketNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Created = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Served = table.Column<bool>(type: "bit", nullable: false),
                    CheckInTime = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OrderNumber = table.Column<int>(type: "int", nullable: false),
                    ServiceId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TodaysTickets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TodaysTickets_Services_ServiceId",
                        column: x => x.ServiceId,
                        principalTable: "Services",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TodaysTickets_ServiceId",
                table: "TodaysTickets",
                column: "ServiceId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TodaysTickets");
        }
    }
}
