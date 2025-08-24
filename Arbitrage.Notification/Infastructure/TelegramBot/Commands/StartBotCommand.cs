using Arbitrage.Domain.TelegramBot;
using Arbitrage.Notification.Application.Contracts;
using Arbitrage.Notification.Domain.Entities;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace Arbitrage.Notification.Infastructure.TelegramBot.Commands
{
    public class StartBotCommand(
        ITelegramUserSettingsService settingsService
    ) : ITelegramCommand
    {
        private readonly ITelegramUserSettingsService _settingsService = settingsService;

        public string Name { get; } = CommandNames.Start;

        public async Task Execute(Update update, ITelegramBotClient botClient, CancellationToken ct)
        {
            var chatId = update.Message!.Chat.Id;

            await _settingsService.CreateOrUpdateSettingsAsync("", chatId.ToString(), NotificationTelegramState.AwaitingUsername);

            await botClient.SendMessage(
                chatId: update.Message!.Chat.Id,
                text: "😊 Привет! Пожалуйста, введите ваш ник на сайте, для привязки уведомлений:",
                cancellationToken: ct
                );
        }
    }
}