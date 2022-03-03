using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CRPL.Web.Migrations.Application
{
    public partial class workUsage : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "LastPing",
                table: "RegisteredWorks",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastProxyUse",
                table: "RegisteredWorks",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "TimesPinged",
                table: "RegisteredWorks",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "TimesProxyUsed",
                table: "RegisteredWorks",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LastPing",
                table: "RegisteredWorks");

            migrationBuilder.DropColumn(
                name: "LastProxyUse",
                table: "RegisteredWorks");

            migrationBuilder.DropColumn(
                name: "TimesPinged",
                table: "RegisteredWorks");

            migrationBuilder.DropColumn(
                name: "TimesProxyUsed",
                table: "RegisteredWorks");
        }
    }
}
