using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CRPL.Web.Migrations.Application
{
    public partial class transactionIds : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ProposalTransactionId",
                table: "RegisteredWorks",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "TransactionId",
                table: "Applications",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProposalTransactionId",
                table: "RegisteredWorks");

            migrationBuilder.DropColumn(
                name: "TransactionId",
                table: "Applications");
        }
    }
}
