using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Arbitrage.WebApi.Migrations
{
    /// <inheritdoc />
    public partial class SymbolRenameToCoinInDb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CurrencyPairs_Symbols_BaseCoinId",
                table: "CurrencyPairs");

            migrationBuilder.DropForeignKey(
                name: "FK_CurrencyPairs_Symbols_QuoteCoinId",
                table: "CurrencyPairs");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Symbols",
                table: "Symbols");

            migrationBuilder.RenameTable(
                name: "Symbols",
                newName: "Coins");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Coins",
                table: "Coins",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CurrencyPairs_Coins_BaseCoinId",
                table: "CurrencyPairs",
                column: "BaseCoinId",
                principalTable: "Coins",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CurrencyPairs_Coins_QuoteCoinId",
                table: "CurrencyPairs",
                column: "QuoteCoinId",
                principalTable: "Coins",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CurrencyPairs_Coins_BaseCoinId",
                table: "CurrencyPairs");

            migrationBuilder.DropForeignKey(
                name: "FK_CurrencyPairs_Coins_QuoteCoinId",
                table: "CurrencyPairs");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Coins",
                table: "Coins");

            migrationBuilder.RenameTable(
                name: "Coins",
                newName: "Symbols");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Symbols",
                table: "Symbols",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CurrencyPairs_Symbols_BaseCoinId",
                table: "CurrencyPairs",
                column: "BaseCoinId",
                principalTable: "Symbols",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CurrencyPairs_Symbols_QuoteCoinId",
                table: "CurrencyPairs",
                column: "QuoteCoinId",
                principalTable: "Symbols",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
