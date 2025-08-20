using Arbitrage.Notification.Domain.Contracts;
using Arbitrage.Notification.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Arbitrage.Notification.Infastructure.Repositories
{
    public class NotificationRepository(DbContext context) : INotificationRepository
    {
        public async Task<NotificationModel> CreateAsync(NotificationModel notification)
        {
            if (notification.Id == Guid.Empty)
                notification.Id = Guid.NewGuid();

            context.Set<NotificationModel>().Update(notification);
            await context.Set<NotificationModel>().AddAsync(notification);
            await context.SaveChangesAsync();
            return notification;
        }

        public async Task<bool> UpdateAsync(NotificationModel notification)
        {
            context.Entry(notification).State = EntityState.Modified;
            return await context.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var notification = await context.Set<NotificationModel>().FindAsync(id);
            if (notification == null) return false;

            context.Set<NotificationModel>().Remove(notification);
            return await context.SaveChangesAsync() > 0;
        }

        public async Task<NotificationModel> GetByIdAsync(Guid id)
            => await context.Set<NotificationModel>().FindAsync(id) ?? new NotificationModel();

        public async Task<IEnumerable<NotificationModel>> GetByUserIdAsync(Guid userId)
            => await context.Set<NotificationModel>()
                .Where(n => n.UserId == userId)
                .ToListAsync();

        public async Task<IEnumerable<NotificationModel>> GetByCurrencyPairIdAsync(Guid currencyPairId)
            => await context.Set<NotificationModel>()
                .Where(n => n.CurrencyPairId == currencyPairId)
                .ToListAsync();
    }
}