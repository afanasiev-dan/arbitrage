using Arbitrage.Symbols.CoinNames.Exchanges;

namespace Arbitrage.Symbols.PairNames.Exchanges.what
{
    public static class MarketPairApiFactory
    {
        public static IMarketPairApi GetExchangeApi(string exchange)
        {
            return exchange switch
            {
                ExchangeDomain.Exchanges.ByBit => new ByBit(),
                ExchangeDomain.Exchanges.KuCoin => new KuCoin(),
                ExchangeDomain.Exchanges.Gate => new Gate(),
                ExchangeDomain.Exchanges.Mexc => new Mexc(),
                ExchangeDomain.Exchanges.Htx => new Htx(),
                ExchangeDomain.Exchanges.Binance => new Binance(),
                ExchangeDomain.Exchanges.LBank => new Lbank(),
                _ => throw new ArgumentException($"Не поддерживается: {exchange}")
            };
        }
    }
}
