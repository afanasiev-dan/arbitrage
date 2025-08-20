namespace Arbitrage.Notification.Presentation.Dto
{
    public class UpdateNotificationDto
    {
        public Guid Id { get; set; }
        public string? Description { get; set; }
        public decimal TargetPrice { get; set; }
    }
}