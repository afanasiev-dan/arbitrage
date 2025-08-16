using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Arbitrage.WebApi.Migrations
{
    /// <inheritdoc />
    public partial class AddedExchanges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Name",
                table: "ExchangeFormats");

            migrationBuilder.AddColumn<int>(
                name: "NameId",
                table: "ExchangeFormats",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "ExchangeModel",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExchangeModel", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ExchangeFormats_NameId",
                table: "ExchangeFormats",
                column: "NameId");

            migrationBuilder.AddForeignKey(
                name: "FK_ExchangeFormats_ExchangeModel_NameId",
                table: "ExchangeFormats",
                column: "NameId",
                principalTable: "ExchangeModel",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ExchangeFormats_ExchangeModel_NameId",
                table: "ExchangeFormats");

            migrationBuilder.DropTable(
                name: "ExchangeModel");

            migrationBuilder.DropIndex(
                name: "IX_ExchangeFormats_NameId",
                table: "ExchangeFormats");

            migrationBuilder.DropColumn(
                name: "NameId",
                table: "ExchangeFormats");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "ExchangeFormats",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }
    }
}
