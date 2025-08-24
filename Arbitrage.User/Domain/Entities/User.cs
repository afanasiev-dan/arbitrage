using System.ComponentModel.DataAnnotations.Schema;
using Arbitrage.Domain.TelegramBot.Entities;

namespace Arbitrage.User.Domain.Entities
{
    [Table("Users")]
    public class UserModel
    {
        public Guid Id { get; set; }
        public required string Username { get; set; }
        public required string PasswordHash { get; set; }
        public required string Email { get; set; }
        public string Role { get; set; } = "User";
        public Guid? TelegramUserSettingsId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime LastActive { get; set; } = DateTime.UtcNow;

        public virtual TelegramUserSettings? TelegramUserSettings { get; set; }
    }
}