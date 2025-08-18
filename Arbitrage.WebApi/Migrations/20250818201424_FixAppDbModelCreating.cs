using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Arbitrage.WebApi.Migrations
{
    /// <inheritdoc />
    public partial class FixAppDbModelCreating : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ScanerData_Exchanges_ExchangeLongId",
                table: "ScanerData");

            migrationBuilder.DropIndex(
                name: "IX_ScanerData_ExchangeLongId",
                table: "ScanerData");

            migrationBuilder.DropColumn(
                name: "ExchangeLongId",
                table: "ScanerData");

            migrationBuilder.CreateIndex(
                name: "IX_ScanerData_ExchangeIdShort",
                table: "ScanerData",
                column: "ExchangeIdShort");

            migrationBuilder.AddForeignKey(
                name: "FK_ScanerData_Exchanges_ExchangeIdShort",
                table: "ScanerData",
                column: "ExchangeIdShort",
                principalTable: "Exchanges",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ScanerData_Exchanges_ExchangeIdShort",
                table: "ScanerData");

            migrationBuilder.DropIndex(
                name: "IX_ScanerData_ExchangeIdShort",
                table: "ScanerData");

            migrationBuilder.AddColumn<Guid>(
                name: "ExchangeLongId",
                table: "ScanerData",
                type: "TEXT",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ScanerData_ExchangeLongId",
                table: "ScanerData",
                column: "ExchangeLongId");

            migrationBuilder.AddForeignKey(
                name: "FK_ScanerData_Exchanges_ExchangeLongId",
                table: "ScanerData",
                column: "ExchangeLongId",
                principalTable: "Exchanges",
                principalColumn: "Id");
        }
    }
}
