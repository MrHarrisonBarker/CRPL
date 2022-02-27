using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CRPL.Web.Migrations.Application
{
    public partial class ApplicationApplicationRef : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ExpectedRecourseApplicationId",
                table: "Applications",
                type: "char(36)",
                nullable: true,
                collation: "ascii_general_ci");

            migrationBuilder.AddColumn<Guid>(
                name: "OriginId",
                table: "Applications",
                type: "char(36)",
                nullable: true,
                collation: "ascii_general_ci");

            migrationBuilder.AddColumn<int>(
                name: "RestructureReason",
                table: "Applications",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Applications_ExpectedRecourseApplicationId",
                table: "Applications",
                column: "ExpectedRecourseApplicationId");

            migrationBuilder.CreateIndex(
                name: "IX_Applications_OriginId",
                table: "Applications",
                column: "OriginId");

            migrationBuilder.AddForeignKey(
                name: "FK_Applications_Applications_ExpectedRecourseApplicationId",
                table: "Applications",
                column: "ExpectedRecourseApplicationId",
                principalTable: "Applications",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Applications_Applications_OriginId",
                table: "Applications",
                column: "OriginId",
                principalTable: "Applications",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Applications_Applications_ExpectedRecourseApplicationId",
                table: "Applications");

            migrationBuilder.DropForeignKey(
                name: "FK_Applications_Applications_OriginId",
                table: "Applications");

            migrationBuilder.DropIndex(
                name: "IX_Applications_ExpectedRecourseApplicationId",
                table: "Applications");

            migrationBuilder.DropIndex(
                name: "IX_Applications_OriginId",
                table: "Applications");

            migrationBuilder.DropColumn(
                name: "ExpectedRecourseApplicationId",
                table: "Applications");

            migrationBuilder.DropColumn(
                name: "OriginId",
                table: "Applications");

            migrationBuilder.DropColumn(
                name: "RestructureReason",
                table: "Applications");
        }
    }
}
