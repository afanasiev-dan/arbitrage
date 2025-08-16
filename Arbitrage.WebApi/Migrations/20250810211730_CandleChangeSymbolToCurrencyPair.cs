using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Arbitrage.WebApi.Migrations
{
    /// <inheritdoc />
    public partial class CandleChangeSymbolToCurrencyPair : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Candles_Symbols_SymbolId",
                table: "Candles");

            migrationBuilder.RenameColumn(
                name: "SymbolId",
                table: "Candles",
                newName: "CurrencyPairId");

            migrationBuilder.RenameIndex(
                name: "IX_Candles_SymbolId",
                table: "Candles",
                newName: "IX_Candles_CurrencyPairId");

            migrationBuilder.AddForeignKey(
                name: "FK_Candles_CurrencyPairs_CurrencyPairId",
                table: "Candles",
                column: "CurrencyPairId",
                principalTable: "CurrencyPairs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Candles_CurrencyPairs_CurrencyPairId",
                table: "Candles");

            migrationBuilder.RenameColumn(
                name: "CurrencyPairId",
                table: "Candles",
                newName: "SymbolId");

            migrationBuilder.RenameIndex(
                name: "IX_Candles_CurrencyPairId",
                table: "Candles",
                newName: "IX_Candles_SymbolId");

            migrationBuilder.AddForeignKey(
                name: "FK_Candles_Symbols_SymbolId",
                table: "Candles",
                column: "SymbolId",
                principalTable: "Symbols",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
