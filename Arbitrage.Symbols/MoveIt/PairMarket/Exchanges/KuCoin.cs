using Arbitrage.ExchangeDomain;
using Arbitrage.ExchangeDomain.Enums;
using Arbitrage.Symbols.CoinNames.Alias;
using Arbitrage.Symbols.PairNames.Exchanges.what;
using Arbitrage.Test.CoinNames;
using System.Text.Json;

namespace Arbitrage.Symbols.CoinNames.Exchanges
{
    public class KuCoin : IMarketPairApi
    {
        private record ApiConfig(
            string Url
        );

        private static readonly Dictionary<MarketType, ApiConfig> Configs = new()
        {
            [MarketType.Spot] = new(
                Url: "https://api.kucoin.com/api/v2/symbols"
            ),
            [MarketType.Futures] = new(
                Url: "https://api-futures.kucoin.com/api/v1/contracts/active"
            )
        };

        public async Task<List<MarketPair>> GetPairs(MarketType marketType)
        {
            if (!Configs.TryGetValue(marketType, out var config))
                throw new ArgumentException("Unsupported market type");

            var response = await Network.GetAsync(config.Url);
            var result = new List<MarketPair>();

            using var doc = JsonDocument.Parse(response);
            var root = doc.RootElement;

            foreach (var item in root.GetProperty("data").EnumerateArray())
            {
                var pair = new MarketPair
                {
                    Ticker = item.GetProperty("symbol").GetString(),
                    Base = item.GetProperty("baseCurrency").GetString(),
                    Quote = item.GetProperty("quoteCurrency").GetString(),
                };
                pair.Exchange = ExchangeDomain.Exchanges.KuCoin;
                pair.MarketType = marketType;
                pair.Original = AliasHelper.GetOriginalName(pair);
                result.Add(pair);
            }

            return result;
        }
    }
}
