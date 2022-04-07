using Microsoft.EntityFrameworkCore.Migrations;

namespace HubnyxQMS.Data.Migrations
{
    public partial class ticket : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Tickets",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    TicketReports = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RegisteredDevice = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Created = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ServedMembers = table.Column<int>(type: "int", nullable: false),
                    NoTurnOutMembers = table.Column<int>(type: "int", nullable: false),
                    Ratings = table.Column<int>(type: "int", nullable: false),
                    TotalEscalated = table.Column<int>(type: "int", nullable: false),
                    AverageWaitingTime = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AverageServingTime = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MembersBeingServed = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tickets", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Tickets");
        }
    }
}
