using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CRPL.Web.Migrations.Application
{
    public partial class dispute : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ContactAddress",
                table: "Applications",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<int>(
                name: "DisputeType",
                table: "Applications",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DisputingWallet",
                table: "Applications",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "ExpectedRecourse",
                table: "Applications",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<int>(
                name: "Infractions",
                table: "Applications",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LinkToInfraction",
                table: "Applications",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "Reason",
                table: "Applications",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<DateTime>(
                name: "Spotted",
                table: "Applications",
                type: "datetime(6)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ContactAddress",
                table: "Applications");

            migrationBuilder.DropColumn(
                name: "DisputeType",
                table: "Applications");

            migrationBuilder.DropColumn(
                name: "DisputingWallet",
                table: "Applications");

            migrationBuilder.DropColumn(
                name: "ExpectedRecourse",
                table: "Applications");

            migrationBuilder.DropColumn(
                name: "Infractions",
                table: "Applications");

            migrationBuilder.DropColumn(
                name: "LinkToInfraction",
                table: "Applications");

            migrationBuilder.DropColumn(
                name: "Reason",
                table: "Applications");

            migrationBuilder.DropColumn(
                name: "Spotted",
                table: "Applications");
        }
    }
}
