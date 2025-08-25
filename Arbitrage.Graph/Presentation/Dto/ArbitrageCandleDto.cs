namespace Arbitrage.Graph.Presentation.Dto
{
    public class ArbitrageCandleDto
    {
        public Guid Id { get; set; }
        public DateTime OpenTime { get; set; }
        public int Interval { get; set; }
        public decimal Open { get; set; }
        public decimal High { get; set; }
        public decimal Low { get; set; }
        public decimal Close { get; set; }
        public string BaseCoinName { get; set; }
        public string QuoteCoinName { get; set; } = "USDT";
        public string ExchangeLongName { get; set; } = string.Empty;
        public string? MarketTypeLong { get; set; }
        public string ExchangeShortName { get; set; } = string.Empty;
        public string? MarketTypeShort { get; set; }
    }
}