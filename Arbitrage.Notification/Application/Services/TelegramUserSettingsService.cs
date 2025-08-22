using Arbitrage.Domain.TelegramBot.Entities;
using Arbitrage.Notification.Application.Contracts;
using Arbitrage.Notification.Domain.Contracts;
using Arbitrage.Notification.Domain.Entities;
using Arbitrage.User.Application.Contracts;
using Microsoft.Extensions.Logging;
using Telegram.Bot;

namespace Arbitrage.Notification.Infastructure.TelegramBot
{
    public class TelegramUserSettingsService(
        IUserService userService,
        ITelegramBotClient telegramBotClient,
        INotificationService notificationService,
        ITelegramUserSettingsRepository repository,
        ILogger<TelegramUserSettingsService> logger
    ) : ITelegramUserSettingsService
    {
        private readonly IUserService _userService = userService;
        private readonly ITelegramBotClient _telegramBotClient = telegramBotClient;
        private readonly INotificationService _notificationService = notificationService;
        private readonly ITelegramUserSettingsRepository _repository = repository;
        private readonly ILogger<TelegramUserSettingsService> _logger = logger;


        public async Task<TelegramUserSettings> GetSettingsByUsernameAsync(string username)
        {
            return await _repository.GetByUsernameAsync(username);
        }

        public async Task<TelegramUserSettings> GetSettingsByChatIdAsync(string chatId)
        {
            return await _repository.GetByChatIdAsync(chatId);
        }

        public async Task<TelegramUserSettings> CreateOrUpdateSettingsAsync(string username, string chatId, string state, string stateData = "")
        {
            if (!string.IsNullOrEmpty(username))
            {
                var existingSettings = await _repository.GetByUsernameAsync(username);

                if (existingSettings != null)
                {
                    // Обновляем существующие настройки
                    existingSettings.Username = username;
                    existingSettings.ChatId = chatId;
                    existingSettings.State = state;
                    existingSettings.StateData = stateData;
                    existingSettings.UpdateAt = DateTime.UtcNow;

                    await _repository.UpdateAsync(existingSettings);
                    return existingSettings;
                }
            }

            // Создаем новые настройки
            var newSettings = new TelegramUserSettings
            {
                Id = Guid.NewGuid(),
                Username = username,
                ChatId = chatId,
                State = state,
                StateData = stateData,
                CreatedAt = DateTime.UtcNow,
                UpdateAt = DateTime.UtcNow
            };

            await _repository.AddAsync(newSettings);
            return newSettings;
        }

        public async Task UpdateStateAsync(string username, string state, string stateData = "")
        {
            var settings = await _repository.GetByUsernameAsync(username);

            if (settings != null)
            {
                settings.State = state;
                settings.StateData = stateData;
                settings.UpdateAt = DateTime.UtcNow;

                await _repository.UpdateAsync(settings);
            }
        }

        public async Task<bool> DeleteSettingsAsync(string username)
        {
            var settings = await _repository.GetByUsernameAsync(username);

            if (settings != null)
            {
                await _repository.DeleteAsync(settings.Id);
                return true;
            }

            return false;
        }
    }
}