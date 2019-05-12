using Microsoft.EntityFrameworkCore.Migrations;

namespace JustGo.Migrations
{
    public partial class MySql : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "Places",
                nullable: false,
                oldClrType: typeof(int))
                .OldAnnotation("MySQL:AutoIncrement", true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "Places",
                nullable: false,
                oldClrType: typeof(int))
                .Annotation("MySQL:AutoIncrement", true);
        }
    }
}
