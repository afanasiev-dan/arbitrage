using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Arbitrage.WebApi.Migrations
{
    /// <inheritdoc />
    public partial class ChangeCurrencyPair : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CurrencyPairs_Symbols_BaseCoinId1",
                table: "CurrencyPairs");

            migrationBuilder.DropForeignKey(
                name: "FK_CurrencyPairs_Symbols_QuoteCoinId1",
                table: "CurrencyPairs");

            migrationBuilder.DropIndex(
                name: "IX_CurrencyPairs_BaseCoinId1",
                table: "CurrencyPairs");

            migrationBuilder.DropColumn(
                name: "BaseCoinId1",
                table: "CurrencyPairs");

            migrationBuilder.RenameColumn(
                name: "QuoteCoinId1",
                table: "CurrencyPairs",
                newName: "ExchangeId");

            migrationBuilder.RenameIndex(
                name: "IX_CurrencyPairs_QuoteCoinId1",
                table: "CurrencyPairs",
                newName: "IX_CurrencyPairs_ExchangeId");

            migrationBuilder.AlterColumn<Guid>(
                name: "QuoteCoinId",
                table: "CurrencyPairs",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AlterColumn<Guid>(
                name: "BaseCoinId",
                table: "CurrencyPairs",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "CurrencyPairs",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER")
                .OldAnnotation("Sqlite:Autoincrement", true);

            migrationBuilder.AddColumn<int>(
                name: "MarketType",
                table: "CurrencyPairs",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "exchangeType",
                table: "CurrencyPairs",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_CurrencyPairs_BaseCoinId",
                table: "CurrencyPairs",
                column: "BaseCoinId");

            migrationBuilder.CreateIndex(
                name: "IX_CurrencyPairs_QuoteCoinId",
                table: "CurrencyPairs",
                column: "QuoteCoinId");

            migrationBuilder.AddForeignKey(
                name: "FK_CurrencyPairs_Exchanges_ExchangeId",
                table: "CurrencyPairs",
                column: "ExchangeId",
                principalTable: "Exchanges",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CurrencyPairs_Exchanges_ExchangeId",
                table: "CurrencyPairs");

            migrationBuilder.DropForeignKey(
                name: "FK_CurrencyPairs_Symbols_BaseCoinId",
                table: "CurrencyPairs");

            migrationBuilder.DropForeignKey(
                name: "FK_CurrencyPairs_Symbols_QuoteCoinId",
                table: "CurrencyPairs");

            migrationBuilder.DropIndex(
                name: "IX_CurrencyPairs_BaseCoinId",
                table: "CurrencyPairs");

            migrationBuilder.DropIndex(
                name: "IX_CurrencyPairs_QuoteCoinId",
                table: "CurrencyPairs");

            migrationBuilder.DropColumn(
                name: "MarketType",
                table: "CurrencyPairs");

            migrationBuilder.DropColumn(
                name: "exchangeType",
                table: "CurrencyPairs");

            migrationBuilder.RenameColumn(
                name: "ExchangeId",
                table: "CurrencyPairs",
                newName: "QuoteCoinId1");

            migrationBuilder.RenameIndex(
                name: "IX_CurrencyPairs_ExchangeId",
                table: "CurrencyPairs",
                newName: "IX_CurrencyPairs_QuoteCoinId1");

            migrationBuilder.AlterColumn<int>(
                name: "QuoteCoinId",
                table: "CurrencyPairs",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<int>(
                name: "BaseCoinId",
                table: "CurrencyPairs",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "CurrencyPairs",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "TEXT")
                .Annotation("Sqlite:Autoincrement", true);

            migrationBuilder.AddColumn<Guid>(
                name: "BaseCoinId1",
                table: "CurrencyPairs",
                type: "TEXT",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_CurrencyPairs_BaseCoinId1",
                table: "CurrencyPairs",
                column: "BaseCoinId1");

            migrationBuilder.AddForeignKey(
                name: "FK_CurrencyPairs_Symbols_BaseCoinId1",
                table: "CurrencyPairs",
                column: "BaseCoinId1",
                principalTable: "Symbols",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CurrencyPairs_Symbols_QuoteCoinId1",
                table: "CurrencyPairs",
                column: "QuoteCoinId1",
                principalTable: "Symbols",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
