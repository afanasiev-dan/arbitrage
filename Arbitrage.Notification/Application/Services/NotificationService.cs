using Arbitrage.Notification.Domain.Contracts;
using Arbitrage.Notification.Domain.Entities;
using Arbitrage.Notification.Presentation.Dto;

namespace Arbitrage.Notification
{
    public class NotificationService(INotificationRepository repository) : INotificationService
    {
        private readonly INotificationRepository _repository = repository;

        public async Task<NotificationModel> CreateNotificationAsync(CreateNotificationDto dto)
        {
            var notification = new NotificationModel
            {
                UserId = dto.UserId,
                Description = dto.Description,
                CurrencyPairId = dto.CurrencyPairId,
                TargetPrice = dto.TargetPrice
            };

            return await _repository.CreateAsync(notification);
        }

        public async Task<bool> UpdateNotificationAsync(UpdateNotificationDto dto)
        {
            var notification = await _repository.GetByIdAsync(dto.Id);
            if (notification == null) return false;

            notification.Description = dto.Description;
            notification.TargetPrice = dto.TargetPrice;

            return await _repository.UpdateAsync(notification);
        }

        public async Task<bool> DeleteNotificationAsync(Guid id)
            => await _repository.DeleteAsync(id);

        public Task<NotificationModel> GetNotificationAsync(Guid id)
            => _repository.GetByIdAsync(id);

        public Task<IEnumerable<NotificationModel>> GetUserNotificationsAsync(Guid userId)
            => _repository.GetByUserIdAsync(userId);

        public Task<IEnumerable<NotificationModel>> GetCurrencyPairNotificationsAsync(Guid currencyPairId)
            => _repository.GetByCurrencyPairIdAsync(currencyPairId);
    }
}