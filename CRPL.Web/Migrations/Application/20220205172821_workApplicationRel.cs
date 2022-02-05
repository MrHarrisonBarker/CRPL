using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CRPL.Web.Migrations.Application
{
    public partial class workApplicationRel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "Registered",
                table: "RegisteredWorks",
                type: "datetime(6)",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)");

            migrationBuilder.AddColumn<Guid>(
                name: "AssociatedWorkId",
                table: "Applications",
                type: "char(36)",
                nullable: true,
                collation: "ascii_general_ci");

            migrationBuilder.CreateIndex(
                name: "IX_Applications_AssociatedWorkId",
                table: "Applications",
                column: "AssociatedWorkId");

            migrationBuilder.AddForeignKey(
                name: "FK_Applications_RegisteredWorks_AssociatedWorkId",
                table: "Applications",
                column: "AssociatedWorkId",
                principalTable: "RegisteredWorks",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Applications_RegisteredWorks_AssociatedWorkId",
                table: "Applications");

            migrationBuilder.DropIndex(
                name: "IX_Applications_AssociatedWorkId",
                table: "Applications");

            migrationBuilder.DropColumn(
                name: "AssociatedWorkId",
                table: "Applications");

            migrationBuilder.AlterColumn<DateTime>(
                name: "Registered",
                table: "RegisteredWorks",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldNullable: true);
        }
    }
}
