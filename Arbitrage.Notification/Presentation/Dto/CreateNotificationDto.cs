namespace Arbitrage.Notification.Presentation.Dto
{
    public class CreateNotificationDto
    {
        public Guid UserId { get; set; }
        public string? Description { get; set; }
        public Guid CurrencyPairId { get; set; }
        public decimal TargetPrice { get; set; }
    }
}