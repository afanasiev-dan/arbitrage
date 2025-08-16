namespace Arbitrage.Graph.Infastructure.ExchangeApiClients
{
    public abstract class CandleResponceDtoBase
    {
        /// <summary>
        /// Время открытия Unix Timestamp
        /// </summary>
        public long OpenTime { get; set; }

        /// <summary>
        /// Цена открытия
        /// </summary>
        public string Open { get; set; }

        /// <summary>
        /// Максимальная цена
        /// </summary>
        public string High { get; set; }

        /// <summary>
        /// Минимальная цена
        /// </summary>
        public string Low { get; set; }

        /// <summary>
        /// Цена закрытия
        /// </summary>
        public string Close { get; set; }

        /// <summary>
        /// Объём
        /// </summary>
        public string Volume { get; set; }

    }
}