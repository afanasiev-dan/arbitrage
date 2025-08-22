using Arbitrage.Domain.TelegramBot.Entities;

namespace Arbitrage.Notification.Application.Contracts
{
    public interface ITelegramUserSettingsService
    {
        Task<TelegramUserSettings> GetSettingsByUsernameAsync(string username);
        Task<TelegramUserSettings> GetSettingsByChatIdAsync(string chatId);
        Task<TelegramUserSettings> CreateOrUpdateSettingsAsync(string username, string chatId, string state, string stateData = "");
        Task UpdateStateAsync(string username, string state, string stateData = "");
        Task<bool> DeleteSettingsAsync(string username);
    }
}