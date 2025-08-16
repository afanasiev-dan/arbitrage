using Arbitrage.Exchange.Domain.Entities;
using Arbitrage.ExchangeDomain.Enums;

namespace Arbitrage.Symbols.Domain.Entities
{
    public class CurrencyPair
    {
        public Guid Id { get; set; }
        public string Pair { get; set; }
        public Guid BaseCoinId { get; set; }
        public Guid QuoteCoinId { get; set; }
        public Guid ExchangeId { get; set; }
        public MarketType MarketType { get; set; }
        public ExchangeType exchangeType { get; set; }


        public Coin BaseCoin { get; set; }
        public Coin QuoteCoin { get; set; }
        public ExchangeModel Exchange { get; set; }
    }
}