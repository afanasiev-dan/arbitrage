using System.Windows.Input;
using Arbitrage.Notification.Domain.Contracts;
using Arbitrage.Notification.Infastructure.TelegramBot.Commands;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Arbitrage.Notification.Infastructure.TelegramBot
{
    public class TelegramBotHandler(
        // ITelegramBotClient botClient,
        CommandsManager commandsManager,
        ILogger<TelegramBotHandler> logger
    ) : ITelegramBotHandler
    {
        // private readonly ITelegramBotClient _botClient = botClient;
        private readonly CommandsManager _commandsManager = commandsManager;
        private readonly ILogger<TelegramBotHandler> _logger = logger;

        public async Task HandleUpdateAsync(
            ITelegramBotClient botClient,
            Update update,
            CancellationToken cancellationToken = default)
        {
            try
            {
                if (update.Type != UpdateType.Message || update.Message!.Type != MessageType.Text)
                    return;

                await _commandsManager.ExecuteCommand(update, botClient, cancellationToken);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
            }
        }

        public Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            var errorMessage = exception switch
            {
                ApiRequestException apiRequestException
                  => $"Ошибка Telegram API:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
                _ => exception.ToString()
            };

            Console.WriteLine(errorMessage);
            return Task.CompletedTask;
        }
    }
}