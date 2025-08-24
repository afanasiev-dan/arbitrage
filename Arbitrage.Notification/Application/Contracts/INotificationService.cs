using Arbitrage.Notification.Domain.Entities;
using Arbitrage.Notification.Presentation.Dto;

namespace Arbitrage.Notification
{
    public interface INotificationService
    {
        Task<NotificationModel> CreateNotificationAsync(CreateNotificationDto dto);
        Task<bool> UpdateNotificationAsync(UpdateNotificationDto dto);
        Task<bool> DeleteNotificationAsync(Guid id);
        Task<NotificationModel> GetNotificationAsync(Guid id);
        Task<IEnumerable<NotificationModel>> GetUserNotificationsAsync(Guid userId);
        Task<IEnumerable<NotificationModel>> GetCurrencyPairNotificationsAsync(Guid currencyPairId);
    }
}