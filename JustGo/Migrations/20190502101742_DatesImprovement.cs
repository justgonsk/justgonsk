using Microsoft.EntityFrameworkCore.Migrations;

namespace JustGo.Migrations
{
    public partial class DatesImprovement : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "Events",
                nullable: true,
                oldClrType: typeof(string));

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Events",
                nullable: true,
                oldClrType: typeof(string));

            migrationBuilder.AddColumn<string>(
                name: "BodyText",
                table: "Events",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ScheduledDates",
                table: "Events",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SingleDates",
                table: "Events",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BodyText",
                table: "Events");

            migrationBuilder.DropColumn(
                name: "ScheduledDates",
                table: "Events");

            migrationBuilder.DropColumn(
                name: "SingleDates",
                table: "Events");

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "Events",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Events",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);
        }
    }
}
