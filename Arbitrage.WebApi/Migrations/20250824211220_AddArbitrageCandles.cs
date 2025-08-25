using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Arbitrage.WebApi.Migrations
{
    /// <inheritdoc />
    public partial class AddArbitrageCandles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ArbitrageCandle",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    OpenTime = table.Column<string>(type: "TEXT", nullable: false),
                    Interval = table.Column<int>(type: "INTEGER", nullable: false),
                    Open = table.Column<decimal>(type: "TEXT", nullable: false),
                    High = table.Column<decimal>(type: "TEXT", nullable: false),
                    Low = table.Column<decimal>(type: "TEXT", nullable: false),
                    Close = table.Column<decimal>(type: "TEXT", nullable: false),
                    ExchangeLongId = table.Column<Guid>(type: "TEXT", nullable: false),
                    MarketTypeLong = table.Column<string>(type: "TEXT", nullable: true),
                    ExchangeShortId = table.Column<Guid>(type: "TEXT", nullable: false),
                    MarketTypeShort = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ArbitrageCandle", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ArbitrageCandle_Exchanges_ExchangeLongId",
                        column: x => x.ExchangeLongId,
                        principalTable: "Exchanges",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ArbitrageCandle_Exchanges_ExchangeShortId",
                        column: x => x.ExchangeShortId,
                        principalTable: "Exchanges",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ArbitrageCandle_ExchangeLongId",
                table: "ArbitrageCandle",
                column: "ExchangeLongId");

            migrationBuilder.CreateIndex(
                name: "IX_ArbitrageCandle_ExchangeShortId",
                table: "ArbitrageCandle",
                column: "ExchangeShortId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ArbitrageCandle");
        }
    }
}
