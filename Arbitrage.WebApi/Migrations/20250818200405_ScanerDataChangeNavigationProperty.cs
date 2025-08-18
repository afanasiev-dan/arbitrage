using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Arbitrage.WebApi.Migrations
{
    /// <inheritdoc />
    public partial class ScanerDataChangeNavigationProperty : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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
                name: "IX_ScanerData_QouteCoinId",
                table: "ScanerData");

            migrationBuilder.DropColumn(
                name: "QouteCoinId",
                table: "ScanerData");

            migrationBuilder.AlterColumn<Guid>(
                name: "ExchangeShortId",
                table: "ScanerData",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<Guid>(
                name: "ExchangeLongId",
                table: "ScanerData",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "TEXT");

            migrationBuilder.CreateIndex(
                name: "IX_ScanerData_QuoteCoinId",
                table: "ScanerData",
                column: "QuoteCoinId");

            migrationBuilder.AddForeignKey(
                name: "FK_ScanerData_Coins_QuoteCoinId",
                table: "ScanerData",
                column: "QuoteCoinId",
                principalTable: "Coins",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ScanerData_Exchanges_ExchangeLongId",
                table: "ScanerData",
                column: "ExchangeLongId",
                principalTable: "Exchanges",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ScanerData_Exchanges_ExchangeShortId",
                table: "ScanerData",
                column: "ExchangeShortId",
                principalTable: "Exchanges",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ScanerData_Coins_QuoteCoinId",
                table: "ScanerData");

            migrationBuilder.DropForeignKey(
                name: "FK_ScanerData_Exchanges_ExchangeLongId",
                table: "ScanerData");

            migrationBuilder.DropForeignKey(
                name: "FK_ScanerData_Exchanges_ExchangeShortId",
                table: "ScanerData");

            migrationBuilder.DropIndex(
                name: "IX_ScanerData_QuoteCoinId",
                table: "ScanerData");

            migrationBuilder.AlterColumn<Guid>(
                name: "ExchangeShortId",
                table: "ScanerData",
                type: "TEXT",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "ExchangeLongId",
                table: "ScanerData",
                type: "TEXT",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "QouteCoinId",
                table: "ScanerData",
                type: "TEXT",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_ScanerData_QouteCoinId",
                table: "ScanerData",
                column: "QouteCoinId");

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
    }
}
