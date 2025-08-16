using Arbitrage.ExchangeDomain.Enums;
using Arbitrage.Test.CoinNames;

namespace Arbitrage.Symbols.PairNames.Exchanges.what
{
    public interface IMarketPairApi
    {
        Task<List<MarketPair>> GetPairs(MarketType marketType);
    }
}
