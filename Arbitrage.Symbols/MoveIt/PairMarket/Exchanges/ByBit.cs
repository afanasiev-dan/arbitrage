using Arbitrage.ExchangeDomain;
using Arbitrage.ExchangeDomain.Enums;
using Arbitrage.Symbols.CoinNames.Alias;
using Arbitrage.Symbols.PairNames.Exchanges.what;
using Arbitrage.Test.CoinNames;
using System.Text.Json;

namespace Arbitrage.Symbols.CoinNames.Exchanges
{
    public class ByBit : IMarketPairApi
    {
        private record ApiConfig(
            string Url
        );

        private static readonly Dictionary<MarketType, ApiConfig> Configs = new()
        {
            [MarketType.Spot] = new(
                Url: "https://api.bybit.com/v5/market/instruments-info?category=spot&status=Trading&limit=1000"
            ),
            [MarketType.Futures] = new(
                Url: "https://api.bybit.com/v5/market/instruments-info?category=linear&status=Trading&limit=1000"
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

            foreach (var item in root.GetProperty("result").GetProperty("list").EnumerateArray())
            {
                if (config.Url.Contains("linear"))
                {
                    var contractType = item.GetProperty("contractType").GetString();
                    if (contractType != "LinearPerpetual")
                        continue;
                }
                var pair = new MarketPair
                {
                    Ticker = item.GetProperty("symbol").GetString(),
                    Base = item.GetProperty("baseCoin").GetString(),
                    Quote = item.GetProperty("quoteCoin").GetString()
                };
                pair.Exchange = ExchangeDomain.Exchanges.ByBit;
                pair.MarketType = marketType;
                pair.Original = AliasHelper.GetOriginalName(pair);
                result.Add(pair);
            }
            return result;
        }
    }
}
