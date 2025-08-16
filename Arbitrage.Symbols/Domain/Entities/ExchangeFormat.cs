using Arbitrage.Exchange.Domain.Entities;

namespace Arbitrage.Symbols.Domain.Entities
{
    public class ExchangeFormat
    {
        public int Id { get; set; }
        public ExchangeModel Name { get; set; } // Binance, Kraken, etc.
        public string? CustomSeparator { get; set; } // Для бирж с фиксированным разделителем
    }
}