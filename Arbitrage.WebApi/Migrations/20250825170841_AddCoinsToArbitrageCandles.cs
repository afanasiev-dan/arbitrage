using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Arbitrage.WebApi.Migrations
{
    /// <inheritdoc />
    public partial class AddCoinsToArbitrageCandles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "BaseCoinId",
                table: "ArbitrageCandle",
                type: "TEXT",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "QuoteCoinId",
                table: "ArbitrageCandle",
                type: "TEXT",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_ArbitrageCandle_BaseCoinId",
                table: "ArbitrageCandle",
                column: "BaseCoinId");

            migrationBuilder.CreateIndex(
                name: "IX_ArbitrageCandle_QuoteCoinId",
                table: "ArbitrageCandle",
                column: "QuoteCoinId");

            migrationBuilder.AddForeignKey(
                name: "FK_ArbitrageCandle_Coins_BaseCoinId",
                table: "ArbitrageCandle",
                column: "BaseCoinId",
                principalTable: "Coins",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ArbitrageCandle_Coins_QuoteCoinId",
                table: "ArbitrageCandle",
                column: "QuoteCoinId",
                principalTable: "Coins",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ArbitrageCandle_Coins_BaseCoinId",
                table: "ArbitrageCandle");

            migrationBuilder.DropForeignKey(
                name: "FK_ArbitrageCandle_Coins_QuoteCoinId",
                table: "ArbitrageCandle");

            migrationBuilder.DropIndex(
                name: "IX_ArbitrageCandle_BaseCoinId",
                table: "ArbitrageCandle");

            migrationBuilder.DropIndex(
                name: "IX_ArbitrageCandle_QuoteCoinId",
                table: "ArbitrageCandle");

            migrationBuilder.DropColumn(
                name: "BaseCoinId",
                table: "ArbitrageCandle");

            migrationBuilder.DropColumn(
                name: "QuoteCoinId",
                table: "ArbitrageCandle");
        }
    }
}
