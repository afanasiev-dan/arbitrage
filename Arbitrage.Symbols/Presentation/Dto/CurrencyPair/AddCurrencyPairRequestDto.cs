using Arbitrage.ExchangeDomain.Enums;

namespace Arbitrage.Symbols.Presentation.Dto.CurrencyPair
{
    public class CurrencyPairRequestDto
    {
        public string Pair { get; set; }
        public string BaseCoinName { get; set; }
        public string QuoteCoinName { get; set; }
        public string ExchangeName { get; set; }
        public MarketType MarketType { get; set; }
        public ExchangeType exchangeType { get; set; }

    }
}