using Arbitrage.Domain.TelegramBot.Entities;
using Arbitrage.Notification.Domain.Entities;

namespace Arbitrage.Notification.Domain.Contracts
{
    public interface ITelegramUserSettingsRepository
    {
        Task<TelegramUserSettings> GetByIdAsync(Guid id);
        Task<TelegramUserSettings> GetByChatIdAsync(string chatId);
        Task<TelegramUserSettings> GetByUsernameAsync(string username);
        Task<IEnumerable<TelegramUserSettings>> GetAllAsync();
        Task AddAsync(TelegramUserSettings settings);
        Task UpdateAsync(TelegramUserSettings settings);
        Task DeleteAsync(Guid id);
    }
}