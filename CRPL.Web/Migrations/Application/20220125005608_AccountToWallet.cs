using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CRPL.Web.Migrations.Application
{
    public partial class AccountToWallet : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "WalletId",
                table: "UserAccounts",
                type: "char(36)",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                collation: "ascii_general_ci");

            migrationBuilder.CreateIndex(
                name: "IX_UserAccounts_WalletId",
                table: "UserAccounts",
                column: "WalletId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserAccounts_UserWallets_WalletId",
                table: "UserAccounts",
                column: "WalletId",
                principalTable: "UserWallets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserAccounts_UserWallets_WalletId",
                table: "UserAccounts");

            migrationBuilder.DropIndex(
                name: "IX_UserAccounts_WalletId",
                table: "UserAccounts");

            migrationBuilder.DropColumn(
                name: "WalletId",
                table: "UserAccounts");
        }
    }
}
