using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace JustGo.Migrations
{
    public partial class Second : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_EventDates",
                table: "EventDates");

            migrationBuilder.AlterColumn<DateTime>(
                name: "Start",
                table: "EventDates",
                nullable: true,
                oldClrType: typeof(DateTime));

            migrationBuilder.AddColumn<int>(
                name: "EventDateId",
                table: "EventDates",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AddPrimaryKey(
                name: "PK_EventDates",
                table: "EventDates",
                column: "EventDateId");

            migrationBuilder.CreateIndex(
                name: "IX_EventDates_EventId",
                table: "EventDates",
                column: "EventId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_EventDates",
                table: "EventDates");

            migrationBuilder.DropIndex(
                name: "IX_EventDates_EventId",
                table: "EventDates");

            migrationBuilder.DropColumn(
                name: "EventDateId",
                table: "EventDates");

            migrationBuilder.AlterColumn<DateTime>(
                name: "Start",
                table: "EventDates",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_EventDates",
                table: "EventDates",
                columns: new[] { "EventId", "Start" });
        }
    }
}
