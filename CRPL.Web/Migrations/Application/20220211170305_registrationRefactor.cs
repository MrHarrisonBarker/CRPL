using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CRPL.Web.Migrations.Application
{
    public partial class registrationRefactor : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CopyrightType",
                table: "Applications");

            migrationBuilder.AddColumn<bool>(
                name: "Protections_Authorship",
                table: "Applications",
                type: "tinyint(1)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Protections_CommercialAdaptation",
                table: "Applications",
                type: "tinyint(1)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Protections_CommercialDistribution",
                table: "Applications",
                type: "tinyint(1)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Protections_CommercialPerformance",
                table: "Applications",
                type: "tinyint(1)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Protections_CommercialReproduction",
                table: "Applications",
                type: "tinyint(1)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Protections_NonCommercialAdaptation",
                table: "Applications",
                type: "tinyint(1)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Protections_NonCommercialDistribution",
                table: "Applications",
                type: "tinyint(1)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Protections_NonCommercialPerformance",
                table: "Applications",
                type: "tinyint(1)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Protections_NonCommercialReproduction",
                table: "Applications",
                type: "tinyint(1)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Protections_ReviewOrCrit",
                table: "Applications",
                type: "tinyint(1)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Protections_Authorship",
                table: "Applications");

            migrationBuilder.DropColumn(
                name: "Protections_CommercialAdaptation",
                table: "Applications");

            migrationBuilder.DropColumn(
                name: "Protections_CommercialDistribution",
                table: "Applications");

            migrationBuilder.DropColumn(
                name: "Protections_CommercialPerformance",
                table: "Applications");

            migrationBuilder.DropColumn(
                name: "Protections_CommercialReproduction",
                table: "Applications");

            migrationBuilder.DropColumn(
                name: "Protections_NonCommercialAdaptation",
                table: "Applications");

            migrationBuilder.DropColumn(
                name: "Protections_NonCommercialDistribution",
                table: "Applications");

            migrationBuilder.DropColumn(
                name: "Protections_NonCommercialPerformance",
                table: "Applications");

            migrationBuilder.DropColumn(
                name: "Protections_NonCommercialReproduction",
                table: "Applications");

            migrationBuilder.DropColumn(
                name: "Protections_ReviewOrCrit",
                table: "Applications");

            migrationBuilder.AddColumn<int>(
                name: "CopyrightType",
                table: "Applications",
                type: "int",
                nullable: true);
        }
    }
}
