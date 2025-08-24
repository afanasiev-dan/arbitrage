using Telegram.Bot;
using Telegram.Bot.Types;

namespace Arbitrage.Notification.Infastructure.TelegramBot.Commands
{
    public interface ITelegramCommand
    {
        string Name { get; }
        Task Execute(Update update, ITelegramBotClient botClient, CancellationToken ct);
    }
}