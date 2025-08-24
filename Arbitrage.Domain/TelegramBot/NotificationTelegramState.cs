namespace Arbitrage.Domain.TelegramBot 
{
    public class NotificationTelegramState
    {
        public const string Start = "start";
        public const string AwaitingUsername = "awaiting_username";
        public const string Completed = "completed";
        public const string Error = "error";
        public const string Paused = "paused";
        
        // Дополнительные методы валидации при необходимости
        public static bool IsValidState(string state)
        {
            return state == Start || state == AwaitingUsername || 
                   state == Completed || state == Error || state == Paused;
        }
    }
}