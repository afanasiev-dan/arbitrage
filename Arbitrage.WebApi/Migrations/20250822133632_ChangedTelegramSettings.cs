using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Arbitrage.WebApi.Migrations
{
    /// <inheritdoc />
    public partial class ChangedTelegramSettings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TelegramId",
                table: "Users",
                newName: "TelegramUserSettingsId");

            migrationBuilder.CreateTable(
                name: "TelegramUserSettings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Username = table.Column<string>(type: "TEXT", nullable: false),
                    ChatId = table.Column<string>(type: "TEXT", nullable: false),
                    State = table.Column<string>(type: "TEXT", nullable: false),
                    StateData = table.Column<string>(type: "TEXT", nullable: false),
                    CreatedAt = table.Column<string>(type: "TEXT", nullable: false),
                    UpdateAt = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TelegramUserSettings", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Users_TelegramUserSettingsId",
                table: "Users",
                column: "TelegramUserSettingsId");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_TelegramUserSettings_TelegramUserSettingsId",
                table: "Users",
                column: "TelegramUserSettingsId",
                principalTable: "TelegramUserSettings",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Users_TelegramUserSettings_TelegramUserSettingsId",
                table: "Users");

            migrationBuilder.DropTable(
                name: "TelegramUserSettings");

            migrationBuilder.DropIndex(
                name: "IX_Users_TelegramUserSettingsId",
                table: "Users");

            migrationBuilder.RenameColumn(
                name: "TelegramUserSettingsId",
                table: "Users",
                newName: "TelegramId");
        }
    }
}
