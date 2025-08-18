using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Arbitrage.WebApi.Migrations
{
    /// <inheritdoc />
    public partial class AddScanerTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ScanerData",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    BaseCoinName = table.Column<Guid>(type: "TEXT", nullable: false),
                    QuoteCoinName = table.Column<Guid>(type: "TEXT", nullable: false),
                    ExchangeNameLong = table.Column<Guid>(type: "TEXT", nullable: false),
                    MarketTypeLong = table.Column<int>(type: "INTEGER", nullable: false),
                    PurchasePriceLong = table.Column<decimal>(type: "TEXT", nullable: false),
                    FundingRateLong = table.Column<decimal>(type: "TEXT", nullable: false),
                    ExchangeNameShort = table.Column<Guid>(type: "TEXT", nullable: false),
                    MarketTypeShort = table.Column<int>(type: "INTEGER", nullable: false),
                    PurchasePriceShort = table.Column<decimal>(type: "TEXT", nullable: false),
                    FundingRateShort = table.Column<decimal>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScanerData", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ScanerData");
        }
    }
}
