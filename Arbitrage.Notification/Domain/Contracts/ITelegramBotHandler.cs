using Telegram.Bot;
using Telegram.Bot.Types;

namespace Arbitrage.Notification.Domain.Contracts
{
    public interface ITelegramBotHandler
    {
        public Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken = default);
        public Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken);
    }
}