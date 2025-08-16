using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Arbitrage.WebApi.Migrations
{
    /// <inheritdoc />
    public partial class SymbolIdIsIntType : Migration
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

            migrationBuilder.DropIndex(
                name: "IX_CurrencyPairs_QuoteCoinId1",
                table: "CurrencyPairs");

            migrationBuilder.DropColumn(
                name: "BaseCoinId1",
                table: "CurrencyPairs");

            migrationBuilder.DropColumn(
                name: "QuoteCoinId1",
                table: "CurrencyPairs");

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "Symbols",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT")
                .Annotation("Sqlite:Autoincrement", true);

            migrationBuilder.CreateIndex(
                name: "IX_CurrencyPairs_BaseCoinId",
                table: "CurrencyPairs",
                column: "BaseCoinId");

            migrationBuilder.CreateIndex(
                name: "IX_CurrencyPairs_QuoteCoinId",
                table: "CurrencyPairs",
                column: "QuoteCoinId");

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

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "Symbols",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER")
                .OldAnnotation("Sqlite:Autoincrement", true);

            migrationBuilder.AddColumn<string>(
                name: "BaseCoinId1",
                table: "CurrencyPairs",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "QuoteCoinId1",
                table: "CurrencyPairs",
                type: "TEXT",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_CurrencyPairs_BaseCoinId1",
                table: "CurrencyPairs",
                column: "BaseCoinId1");

            migrationBuilder.CreateIndex(
                name: "IX_CurrencyPairs_QuoteCoinId1",
                table: "CurrencyPairs",
                column: "QuoteCoinId1");

            migrationBuilder.AddForeignKey(
                name: "FK_CurrencyPairs_Symbols_BaseCoinId1",
                table: "CurrencyPairs",
                column: "BaseCoinId1",
                principalTable: "Symbols",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CurrencyPairs_Symbols_QuoteCoinId1",
                table: "CurrencyPairs",
                column: "QuoteCoinId1",
                principalTable: "Symbols",
                principalColumn: "Id");
        }
    }
}
