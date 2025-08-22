using Arbitrage.Domain.TelegramBot;
using Arbitrage.Notification.Application.Contracts;
using Arbitrage.User.Domain.Contracts;
using Arbitrage.User.Domain.Entities;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace Arbitrage.Notification.Infastructure.TelegramBot.Commands
{
    public class SetUsernameCommand(
        IUserRepository userRepository,
        ITelegramUserSettingsService settingsService,
        ILogger<SetUsernameCommand> logger
    ) : ITelegramCommand
    {
        private readonly IUserRepository _userRepository = userRepository;
        private readonly ITelegramUserSettingsService _settingsService = settingsService;
        private readonly ILogger<SetUsernameCommand> _logger = logger;


        public string Name { get; } = CommandNames.SetUsername;

        public async Task Execute(Update update, ITelegramBotClient botClient, CancellationToken ct)
        {
            var chatId = update.Message!.Chat.Id;
            var username = update.Message!.Text;

            var userSettings = await _settingsService.GetSettingsByChatIdAsync(update.Message.Chat.Id.ToString());

            UserModel user = await _userRepository.GetByUsernameAsync(username);

            if (user is null)
            {
                await botClient.SendMessage(
                    chatId: update.Message!.Chat.Id,
                    text: "😑 Пользователь не найден",
                    cancellationToken: ct);

                return;
            }

            try
            {
                await _settingsService.CreateOrUpdateSettingsAsync(username, chatId.ToString(), NotificationTelegramState.Completed);

                user.TelegramUserSettingsId = userSettings.Id;
                await _userRepository.UpdateAsync(user);

                _logger.LogInformation($"Аккаунт {username} успешно привязан");

                await botClient.SendMessage(
                    chatId,
                    "🎉 Аккаунт успешно привязан! Вы будете получать уведомления.");
            }
            catch (Exception ex)
            {
                _logger.LogError("Ошибка при привязке аккаунта ", ex.Message);

                await botClient.SendMessage(
                    chatId,
                    "😞 Ошибка, попробуйте позже."); 
                throw;
            }

        }
    }
}