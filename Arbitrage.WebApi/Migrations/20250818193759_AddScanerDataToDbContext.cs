using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Arbitrage.WebApi.Migrations
{
    /// <inheritdoc />
    public partial class AddScanerDataToDbContext : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "QuoteCoinName",
                table: "ScanerData",
                newName: "QuoteCoinId");

            migrationBuilder.RenameColumn(
                name: "ExchangeNameShort",
                table: "ScanerData",
                newName: "QouteCoinId");

            migrationBuilder.RenameColumn(
                name: "ExchangeNameLong",
                table: "ScanerData",
                newName: "ExchangeShortId");

            migrationBuilder.RenameColumn(
                name: "BaseCoinName",
                table: "ScanerData",
                newName: "ExchangeLongId");

            migrationBuilder.AddColumn<Guid>(
                name: "BaseCoinId",
                table: "ScanerData",
                type: "TEXT",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "ExchangeIdLong",
                table: "ScanerData",
                type: "TEXT",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "ExchangeIdShort",
                table: "ScanerData",
                type: "TEXT",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_ScanerData_BaseCoinId",
                table: "ScanerData",
                column: "BaseCoinId");

            migrationBuilder.CreateIndex(
                name: "IX_ScanerData_ExchangeLongId",
                table: "ScanerData",
                column: "ExchangeLongId");

            migrationBuilder.CreateIndex(
                name: "IX_ScanerData_ExchangeShortId",
                table: "ScanerData",
                column: "ExchangeShortId");

            migrationBuilder.CreateIndex(
                name: "IX_ScanerData_QouteCoinId",
                table: "ScanerData",
                column: "QouteCoinId");

            migrationBuilder.AddForeignKey(
                name: "FK_ScanerData_Coins_BaseCoinId",
                table: "ScanerData",
                column: "BaseCoinId",
                principalTable: "Coins",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ScanerData_Coins_QouteCoinId",
                table: "ScanerData",
                column: "QouteCoinId",
                principalTable: "Coins",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ScanerData_Exchanges_ExchangeLongId",
                table: "ScanerData",
                column: "ExchangeLongId",
                principalTable: "Exchanges",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ScanerData_Exchanges_ExchangeShortId",
                table: "ScanerData",
                column: "ExchangeShortId",
                principalTable: "Exchanges",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ScanerData_Coins_BaseCoinId",
                table: "ScanerData");

            migrationBuilder.DropForeignKey(
                name: "FK_ScanerData_Coins_QouteCoinId",
                table: "ScanerData");

            migrationBuilder.DropForeignKey(
                name: "FK_ScanerData_Exchanges_ExchangeLongId",
                table: "ScanerData");

            migrationBuilder.DropForeignKey(
                name: "FK_ScanerData_Exchanges_ExchangeShortId",
                table: "ScanerData");

            migrationBuilder.DropIndex(
                name: "IX_ScanerData_BaseCoinId",
                table: "ScanerData");

            migrationBuilder.DropIndex(
                name: "IX_ScanerData_ExchangeLongId",
                table: "ScanerData");

            migrationBuilder.DropIndex(
                name: "IX_ScanerData_ExchangeShortId",
                table: "ScanerData");

            migrationBuilder.DropIndex(
                name: "IX_ScanerData_QouteCoinId",
                table: "ScanerData");

            migrationBuilder.DropColumn(
                name: "BaseCoinId",
                table: "ScanerData");

            migrationBuilder.DropColumn(
                name: "ExchangeIdLong",
                table: "ScanerData");

            migrationBuilder.DropColumn(
                name: "ExchangeIdShort",
                table: "ScanerData");

            migrationBuilder.RenameColumn(
                name: "QuoteCoinId",
                table: "ScanerData",
                newName: "QuoteCoinName");

            migrationBuilder.RenameColumn(
                name: "QouteCoinId",
                table: "ScanerData",
                newName: "ExchangeNameShort");

            migrationBuilder.RenameColumn(
                name: "ExchangeShortId",
                table: "ScanerData",
                newName: "ExchangeNameLong");

            migrationBuilder.RenameColumn(
                name: "ExchangeLongId",
                table: "ScanerData",
                newName: "BaseCoinName");
        }
    }
}
