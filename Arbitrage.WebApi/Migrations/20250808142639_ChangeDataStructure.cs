using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Arbitrage.WebApi.Migrations
{
    /// <inheritdoc />
    public partial class ChangeDataStructure : Migration
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

            migrationBuilder.DropTable(
                name: "ExchangeFormats");

            migrationBuilder.DropIndex(
                name: "IX_CurrencyPairs_BaseCoinId",
                table: "CurrencyPairs");

            migrationBuilder.DropIndex(
                name: "IX_CurrencyPairs_QuoteCoinId",
                table: "CurrencyPairs");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ExchangeModel",
                table: "ExchangeModel");

            migrationBuilder.RenameTable(
                name: "ExchangeModel",
                newName: "Exchanges");

            migrationBuilder.RenameColumn(
                name: "Symbol",
                table: "Candles",
                newName: "SymbolId");

            migrationBuilder.RenameColumn(
                name: "Exchange",
                table: "Candles",
                newName: "ExchangeId");

            migrationBuilder.RenameColumn(
                name: "CloseTime",
                table: "Candles",
                newName: "MarketType");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "Symbols",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER")
                .OldAnnotation("Sqlite:Autoincrement", true);

            migrationBuilder.AddColumn<string>(
                name: "Address",
                table: "Symbols",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "BaseCoinId1",
                table: "CurrencyPairs",
                type: "TEXT",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "QuoteCoinId1",
                table: "CurrencyPairs",
                type: "TEXT",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "Exchanges",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER")
                .OldAnnotation("Sqlite:Autoincrement", true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Exchanges",
                table: "Exchanges",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_CurrencyPairs_BaseCoinId1",
                table: "CurrencyPairs",
                column: "BaseCoinId1");

            migrationBuilder.CreateIndex(
                name: "IX_CurrencyPairs_QuoteCoinId1",
                table: "CurrencyPairs",
                column: "QuoteCoinId1");

            migrationBuilder.CreateIndex(
                name: "IX_Candles_ExchangeId",
                table: "Candles",
                column: "ExchangeId");

            migrationBuilder.CreateIndex(
                name: "IX_Candles_SymbolId",
                table: "Candles",
                column: "SymbolId");

            migrationBuilder.AddForeignKey(
                name: "FK_Candles_Exchanges_ExchangeId",
                table: "Candles",
                column: "ExchangeId",
                principalTable: "Exchanges",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Candles_Symbols_SymbolId",
                table: "Candles",
                column: "SymbolId",
                principalTable: "Symbols",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Candles_Exchanges_ExchangeId",
                table: "Candles");

            migrationBuilder.DropForeignKey(
                name: "FK_Candles_Symbols_SymbolId",
                table: "Candles");

            migrationBuilder.DropForeignKey(
                name: "FK_CurrencyPairs_Symbols_BaseCoinId1",
                table: "CurrencyPairs");

            migrationBuilder.DropForeignKey(
                name: "FK_CurrencyPairs_Symbols_QuoteCoinId1",
                table: "CurrencyPairs");

            migrationBuilder.DropIndex(
                name: "IX_CurrencyPairs_BaseCoinId1",
                table: "CurrencyPairs");

            migrationBuilder.DropIndex(
                name: "IX_CurrencyPairs_QuoteCoinId1",
                table: "CurrencyPairs");

            migrationBuilder.DropIndex(
                name: "IX_Candles_ExchangeId",
                table: "Candles");

            migrationBuilder.DropIndex(
                name: "IX_Candles_SymbolId",
                table: "Candles");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Exchanges",
                table: "Exchanges");

            migrationBuilder.DropColumn(
                name: "Address",
                table: "Symbols");

            migrationBuilder.DropColumn(
                name: "BaseCoinId1",
                table: "CurrencyPairs");

            migrationBuilder.DropColumn(
                name: "QuoteCoinId1",
                table: "CurrencyPairs");

            migrationBuilder.RenameTable(
                name: "Exchanges",
                newName: "ExchangeModel");

            migrationBuilder.RenameColumn(
                name: "SymbolId",
                table: "Candles",
                newName: "Symbol");

            migrationBuilder.RenameColumn(
                name: "MarketType",
                table: "Candles",
                newName: "CloseTime");

            migrationBuilder.RenameColumn(
                name: "ExchangeId",
                table: "Candles",
                newName: "Exchange");

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "Symbols",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "TEXT")
                .Annotation("Sqlite:Autoincrement", true);

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "ExchangeModel",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "TEXT")
                .Annotation("Sqlite:Autoincrement", true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_ExchangeModel",
                table: "ExchangeModel",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "ExchangeFormats",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    NameId = table.Column<int>(type: "INTEGER", nullable: false),
                    CustomSeparator = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExchangeFormats", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ExchangeFormats_ExchangeModel_NameId",
                        column: x => x.NameId,
                        principalTable: "ExchangeModel",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CurrencyPairs_BaseCoinId",
                table: "CurrencyPairs",
                column: "BaseCoinId");

            migrationBuilder.CreateIndex(
                name: "IX_CurrencyPairs_QuoteCoinId",
                table: "CurrencyPairs",
                column: "QuoteCoinId");

            migrationBuilder.CreateIndex(
                name: "IX_ExchangeFormats_NameId",
                table: "ExchangeFormats",
                column: "NameId");

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
