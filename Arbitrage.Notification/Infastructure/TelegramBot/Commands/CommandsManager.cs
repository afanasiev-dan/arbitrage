using Arbitrage.Domain.TelegramBot;
using Arbitrage.Notification.Application.Contracts;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace Arbitrage.Notification.Infastructure.TelegramBot.Commands
{
    public class CommandsManager
    {
        private readonly ITelegramUserSettingsService _settingsService;
        private readonly ILogger<CommandsManager> _logger;
        private readonly List<ITelegramCommand> _commands = new();


        public CommandsManager(
            IEnumerable<ITelegramCommand> telegramCommands,
            ITelegramUserSettingsService settingsService,
            ILogger<CommandsManager> logger)
        {
            _settingsService = settingsService;
            _logger = logger;

            foreach (var command in telegramCommands)
                _commands.Add(command);
        }

        public async Task ExecuteCommand(Update update, ITelegramBotClient botClient, CancellationToken ct)
        {
            string? messageText = update.Message?.Text;

            if (string.IsNullOrEmpty(update.Message?.Text))
                throw new Exception("Сообщение пустое");

            var userSettings = await _settingsService.GetSettingsByChatIdAsync(update.Message.Chat.Id.ToString());

            if (userSettings is not null)
            {
                if (userSettings.State == NotificationTelegramState.AwaitingUsername)
                {
                    var setNameCommand = _commands.FirstOrDefault(c => c.Name == CommandNames.SetUsername);
                    await setNameCommand.Execute(update, botClient, ct);
                }

                await botClient.SendMessage(
                    chatId: userSettings.ChatId,
                    text: "Аккаунт уже привязан",
                    cancellationToken: ct);
            }

            userSettings = await _settingsService.CreateOrUpdateSettingsAsync("", update.Message.Chat.Id.ToString(), NotificationTelegramState.Start);

            if (userSettings.State == NotificationTelegramState.Start)
            {
                var startCommand = _commands.FirstOrDefault(c => c.Name == CommandNames.Start);
                await startCommand.Execute(update, botClient, ct);
                return;
            }

            await botClient.SendMessage(
                chatId: update.Message!.Chat.Id,
                text: "Не понимаю команду 🙈. Попробуйте начать с начала -> /start",
                cancellationToken: ct
                );
        }
    }
}