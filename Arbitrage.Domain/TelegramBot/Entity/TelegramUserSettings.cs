using System.ComponentModel.DataAnnotations.Schema;

namespace Arbitrage.Domain.TelegramBot.Entities
{
    [Table("TelegramUserSettings")]
    public class TelegramUserSettings
    {
        public Guid Id { get; set; }
        public string Username { get; set; }
        public string ChatId { get; set; }

        /// <summary>
        /// Состояние конечного автомата
        /// </summary>
        public string State { get; set; } = NotificationTelegramState.Start;
        
        /// <summary>
        /// Дополнительные данные состояния конечного автомата
        /// </summary>
        public string StateData { get; set; } = "";

        public DateTime CreatedAt { get; set; }
        public DateTime UpdateAt { get; set; }
    }
}