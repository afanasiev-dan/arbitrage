using Arbitrage.Exchange.Domain.Entities;
using Arbitrage.ExchangeDomain.Enums;
using Arbitrage.Symbols.Domain.Entities;

namespace Arbitrage.Scaner.Domain.Entities
{
    public class ScanerModel
    {
        public Guid Id { get; set; }
        public Guid BaseCoinId { get; set; }
        public Guid QuoteCoinId { get; set; }

        public Guid ExchangeIdLong { get; set; }
        public MarketType MarketTypeLong { get; set; }
        public decimal PurchasePriceLong { get; set; }
        public decimal FundingRateLong { get; set; }
        public Guid TickerLongId { get; set; }

        public Guid ExchangeIdShort { get; set; }
        public MarketType MarketTypeShort { get; set; }
        public decimal PurchasePriceShort { get; set; }
        public decimal FundingRateShort { get; set; }
        public Guid TickerShortId { get; set; }

        // Навигационные свойства
        public CurrencyPair? TickerLong { get; set; }
        public CurrencyPair? TickerShort { get; set; }
        public Coin? BaseCoin{ get; set; }
        public Coin? QuoteCoin{ get; set; }
        public ExchangeModel? ExchangeLong{ get; set; }
        public ExchangeModel? ExchangeShort{ get; set; }

    }
}
