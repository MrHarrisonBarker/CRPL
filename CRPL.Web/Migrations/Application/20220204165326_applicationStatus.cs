using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CRPL.Web.Migrations.Application
{
    public partial class applicationStatus : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Applications",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "Applications");
        }
    }
}
