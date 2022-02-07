using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CRPL.Web.Migrations.Application
{
    public partial class transactionIdOnWork : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "RegisteredTransactionId",
                table: "RegisteredWorks",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RegisteredTransactionId",
                table: "RegisteredWorks");
        }
    }
}
