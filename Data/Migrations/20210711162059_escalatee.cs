using Microsoft.EntityFrameworkCore.Migrations;

namespace HubnyxQMS.Data.Migrations
{
    public partial class escalatee : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Escalates",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Reason = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ReplytoMember = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ReplytoEscalator = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EscalatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Created = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedTime = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<bool>(type: "bit", nullable: false),
                    QMSUserId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Escalates", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Escalates_AspNetUsers_QMSUserId",
                        column: x => x.QMSUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Escalates_QMSUserId",
                table: "Escalates",
                column: "QMSUserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Escalates");
        }
    }
}
