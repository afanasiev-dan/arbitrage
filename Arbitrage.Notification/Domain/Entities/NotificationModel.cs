using System.ComponentModel.DataAnnotations.Schema;

namespace Arbitrage.Notification.Domain.Entities
{
    [Table("Notifications")]
    public class NotificationModel
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string? Description { get; set; }
        public Guid CurrencyPairId { get; set; }
        public decimal TargetPrice { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}