namespace Arbitrage.ExchangeDomain.Converters
{
    public static class UnixTimeConverter
    {
        private static readonly DateTime UnixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        /// <summary>
        /// Конвертирует DateTime в Unix timestamp (в миллисекундах).
        /// Автоматически приводит время к UTC.
        /// </summary>
        public static long ToUnixTimeMilliseconds(this DateTime dateTime)
        {
            return (long)(dateTime.ToUniversalTime() - UnixEpoch).TotalMilliseconds;
        }

        /// <summary>
        /// Конвертирует DateTime в Unix timestamp (в секундах).
        /// Автоматически приводит время к UTC.
        /// </summary>
        public static long ToUnixTimeSeconds(this DateTime dateTime)
        {
            return (long)(dateTime.ToUniversalTime() - UnixEpoch).TotalSeconds;
        }

        /// <summary>
        /// Конвертирует Unix timestamp (миллисекунды) обратно в DateTime (UTC).
        /// </summary>
        public static DateTime FromUnixTimeMilliseconds(long milliseconds)
        {
            return UnixEpoch.AddMilliseconds(milliseconds);
        }

        /// <summary>
        /// Конвертирует Unix timestamp (секунды) обратно в DateTime (UTC).
        /// </summary>
        public static DateTime FromUnixTimeSeconds(long seconds)
        {
            return UnixEpoch.AddSeconds(seconds);
        }

        /// <summary>
        /// Возвращает текущий Unix timestamp в миллисекундах (UTC).
        /// </summary>
        public static long CurrentTimeMilliseconds => DateTime.UtcNow.ToUnixTimeMilliseconds();

        /// <summary>
        /// Возвращает текущий Unix timestamp в секундах (UTC).
        /// </summary>
        public static long CurrentTimeSeconds => DateTime.UtcNow.ToUnixTimeSeconds();
    }
}