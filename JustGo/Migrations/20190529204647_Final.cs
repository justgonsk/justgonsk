using Microsoft.EntityFrameworkCore.Migrations;

namespace JustGo.Migrations
{
    public partial class Final : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Events_AspNetUsers_JustGoUserId",
                table: "Events");

            migrationBuilder.DropIndex(
                name: "IX_Events_JustGoUserId",
                table: "Events");

            migrationBuilder.DropColumn(
                name: "JustGoUserId",
                table: "Events");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "JustGoUserId",
                table: "Events",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Events_JustGoUserId",
                table: "Events",
                column: "JustGoUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Events_AspNetUsers_JustGoUserId",
                table: "Events",
                column: "JustGoUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
