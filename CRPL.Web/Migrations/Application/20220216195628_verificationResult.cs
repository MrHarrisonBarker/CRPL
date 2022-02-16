using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CRPL.Web.Migrations.Application
{
    public partial class verificationResult : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "VerificationResult_Collision",
                table: "RegisteredWorks",
                type: "char(36)",
                nullable: true,
                collation: "ascii_general_ci");

            migrationBuilder.AddColumn<bool>(
                name: "VerificationResult_IsAuthentic",
                table: "RegisteredWorks",
                type: "tinyint(1)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "VerificationResult_Collision",
                table: "RegisteredWorks");

            migrationBuilder.DropColumn(
                name: "VerificationResult_IsAuthentic",
                table: "RegisteredWorks");
        }
    }
}
