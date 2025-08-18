using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Arbitrage.WebApi.Migrations
{
    /// <inheritdoc />
    public partial class ChangeAppDbModelCreating : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ScanerData_Exchanges_ExchangeShortId",
                table: "ScanerData");

            migrationBuilder.DropIndex(
                name: "IX_ScanerData_ExchangeShortId",
                table: "ScanerData");

            migrationBuilder.DropColumn(
                name: "ExchangeShortId",
                table: "ScanerData");

            migrationBuilder.CreateIndex(
                name: "IX_ScanerData_ExchangeIdLong",
                table: "ScanerData",
                column: "ExchangeIdLong");

            migrationBuilder.AddForeignKey(
                name: "FK_ScanerData_Exchanges_ExchangeIdLong",
                table: "ScanerData",
                column: "ExchangeIdLong",
                principalTable: "Exchanges",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ScanerData_Exchanges_ExchangeIdLong",
                table: "ScanerData");

            migrationBuilder.DropIndex(
                name: "IX_ScanerData_ExchangeIdLong",
                table: "ScanerData");

            migrationBuilder.AddColumn<Guid>(
                name: "ExchangeShortId",
                table: "ScanerData",
                type: "TEXT",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ScanerData_ExchangeShortId",
                table: "ScanerData",
                column: "ExchangeShortId");

            migrationBuilder.AddForeignKey(
                name: "FK_ScanerData_Exchanges_ExchangeShortId",
                table: "ScanerData",
                column: "ExchangeShortId",
                principalTable: "Exchanges",
                principalColumn: "Id");
        }
    }
}
