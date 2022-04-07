using Microsoft.EntityFrameworkCore.Migrations;

namespace HubnyxQMS.Data.Migrations
{
    public partial class withdrawal : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "WithDrawalReasons",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WithDrawalReasons", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "WithDrawals",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Created = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TicketNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MemberReason = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MemberNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    WithDrawalReasonId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    Status = table.Column<bool>(type: "bit", nullable: false),
                    Chat = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    QMSUserId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WithDrawals", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WithDrawals_AspNetUsers_QMSUserId",
                        column: x => x.QMSUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_WithDrawals_WithDrawalReasons_WithDrawalReasonId",
                        column: x => x.WithDrawalReasonId,
                        principalTable: "WithDrawalReasons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_WithDrawals_QMSUserId",
                table: "WithDrawals",
                column: "QMSUserId");

            migrationBuilder.CreateIndex(
                name: "IX_WithDrawals_WithDrawalReasonId",
                table: "WithDrawals",
                column: "WithDrawalReasonId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "WithDrawals");

            migrationBuilder.DropTable(
                name: "WithDrawalReasons");
        }
    }
}
