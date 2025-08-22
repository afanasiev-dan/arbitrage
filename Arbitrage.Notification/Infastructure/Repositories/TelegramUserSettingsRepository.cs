using Arbitrage.Domain.TelegramBot.Entities;
using Arbitrage.Notification.Domain.Contracts;
using Microsoft.EntityFrameworkCore;

namespace Arbitrage.Notification.Infastructure.Repositories
{
    public class TelegramUserSettingsRepository(DbContext context) : ITelegramUserSettingsRepository
    {
        public async Task<TelegramUserSettings> GetByIdAsync(Guid id)
        {
            return await context.Set<TelegramUserSettings>()
                .FirstOrDefaultAsync(t => t.Id == id);
        }

        public async Task<TelegramUserSettings> GetByChatIdAsync(string chatId)
        {
            return await context.Set<TelegramUserSettings>()
                .FirstOrDefaultAsync(t => t.ChatId == chatId);
        }

        public async Task<TelegramUserSettings> GetByUsernameAsync(string username)
        {
            return await context.Set<TelegramUserSettings>()
                .FirstOrDefaultAsync(t => t.Username == username);
        }

        public async Task<IEnumerable<TelegramUserSettings>> GetAllAsync()
        {
            return await context.Set<TelegramUserSettings>()
                .ToListAsync();
        }

        public async Task AddAsync(TelegramUserSettings settings)
        {
            settings.CreatedAt = DateTime.UtcNow;
            settings.UpdateAt = DateTime.UtcNow;

            await context.Set<TelegramUserSettings>().AddAsync(settings);
            await context.SaveChangesAsync();
        }

        public async Task UpdateAsync(TelegramUserSettings settings)
        {
            settings.UpdateAt = DateTime.UtcNow;

            context.Set<TelegramUserSettings>().Update(settings);
            await context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var settings = await GetByIdAsync(id);
            if (settings != null)
            {
                context.Set<TelegramUserSettings>().Remove(settings);
                await context.SaveChangesAsync();
            }
        }
    }
}