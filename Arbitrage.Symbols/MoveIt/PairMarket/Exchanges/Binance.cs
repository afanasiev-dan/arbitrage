using Arbitrage.ExchangeDomain;
using Arbitrage.ExchangeDomain.Enums;
using Arbitrage.Symbols.CoinNames.Alias;
using Arbitrage.Symbols.PairNames.Exchanges.what;
using Arbitrage.Test.CoinNames;
using System.Text.Json;

namespace Arbitrage.Symbols.CoinNames.Exchanges
{
    public class Binance : IMarketPairApi
    {
        private record ApiConfig(
            string Url,
            Func<JsonElement, bool>? AdditionalFilter = null
        );

        private static readonly Dictionary<MarketType, ApiConfig> Configs = new()
        {
            [MarketType.Spot] = new(
                Url: "https://api.binance.com/api/v3/exchangeInfo"
            ),
            [MarketType.Futures] = new(
                Url: "https://fapi.binance.com/fapi/v1/exchangeInfo",
                AdditionalFilter: (item) =>
                    item.GetProperty("contractType").GetString() == "PERPETUAL"
            )
        };

        public async Task<List<MarketPair>> GetPairs(MarketType marketType)
        {
            if (!Configs.TryGetValue(marketType, out var config))
                throw new ArgumentException("Unsupported market type");

            var response = await Network.GetAsync(config.Url);
            var result = new List<MarketPair>();

            using var doc = JsonDocument.Parse(response);
            var data = doc.RootElement.GetProperty("symbols");

            foreach (var item in data.EnumerateArray())
            {
                if (config.AdditionalFilter != null && !config.AdditionalFilter(item))
                    continue;

                var pair = new MarketPair
                {
                    Ticker = item.GetProperty("symbol").GetString(),
                    Base = item.GetProperty("baseAsset").GetString(),
                    Quote = item.GetProperty("quoteAsset").GetString(),
                    Exchange = ExchangeDomain.Exchanges.Binance,
                    MarketType = marketType
                };

                pair.Original = AliasHelper.GetOriginalName(pair);
                result.Add(pair);
            }
            return result;
        }
    }
}