using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CRPL.Web.Migrations.Application
{
    public partial class deleteAccountApp : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "AccountId",
                table: "Applications",
                type: "char(36)",
                nullable: true,
                collation: "ascii_general_ci");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AccountId",
                table: "Applications");
        }
    }
}
