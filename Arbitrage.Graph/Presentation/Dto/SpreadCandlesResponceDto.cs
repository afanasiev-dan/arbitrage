namespace Arbitrage.Graph.Presentation.Dto
{
    /// <summary>
    /// Информация о графике для спредов свечей
    /// </summary>
    public class SpreadCandlesResponceDto
    {

        /// <summary>
        /// Название биржи лонга
        /// </summary>
        public string ExchangeNameLong { get; set; }

        /// <summary>
        /// Название биржи шорта 
        /// </summary>
        public string ExchangeNameShort { get; set; }

        /// <summary>
        /// Название монеты лонга 
        /// </summary>
        public string SymbolNameLong { get; set; }

        /// <summary>
        /// Название монеты шорта 
        /// </summary>
        public string SymbolNameShort { get; set; } = "USDT";

        /// <summary>
        /// Интвервал свечи
        /// </summary>
        public int Interval { get; set; }

        public required List<CandleDataDto> SpreadCandleData { get; set; }

    }

    /// <summary>
    /// Данные спреда свечи 
    /// </summary>
    public class CandleDataDto
    {
        /// <summary>
        /// Время открытия
        /// </summary>
        public DateTime OpenTime { get; set; }

        /// <summary>
        /// Цена открытия
        /// </summary>
        public decimal Open { get; set; }

        /// <summary>
        /// Максимальная цена
        /// </summary>
        public decimal High { get; set; }

        /// <summary>
        /// Минимальная цена
        /// </summary>
        public decimal Low { get; set; }

        /// <summary>
        /// Цена закрытия
        /// </summary>
        public decimal Close { get; set; }

        /// <summary>
        /// Объём
        /// </summary>
        public decimal Volume { get; set; }
    }
}