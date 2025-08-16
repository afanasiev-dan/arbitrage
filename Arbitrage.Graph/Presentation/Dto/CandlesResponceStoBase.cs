namespace Arbitrage.Graph.Presentation.Dto
{
    public class SpotCandlesResponceDto
    {
        /// <summary>
        /// Индификатор биржи 
        /// </summary>
        public string ExchangeName { get; set; }

        /// <summary>
        /// Индификатор монеты лонга 
        /// </summary>
        public string Symbol { get; set; }

        /// <summary>
        /// Интвервал свечи в секундах
        /// </summary>
        public int Interval { get; set; }

        public required List<CandleDataDto> SpotCandleData { get; set; }
    }
}