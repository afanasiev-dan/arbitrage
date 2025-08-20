using Arbitrage.Notification.Domain.Entities;

namespace Arbitrage.Notification.Domain.Contracts
{
    public interface INotificationRepository
    {
        Task<NotificationModel> CreateAsync(NotificationModel notification);
        Task<bool> UpdateAsync(NotificationModel notification);
        Task<bool> DeleteAsync(Guid id);
        Task<NotificationModel> GetByIdAsync(Guid id);
        Task<IEnumerable<NotificationModel>> GetByUserIdAsync(Guid userId);
        Task<IEnumerable<NotificationModel>> GetByCurrencyPairIdAsync(Guid currencyPairId);
    }
}