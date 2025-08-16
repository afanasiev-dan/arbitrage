using Arbitrage.ExchangeDomain;
using Arbitrage.ExchangeDomain.Enums;
using Arbitrage.Symbols.CoinNames.Alias;
using Arbitrage.Symbols.PairNames.Exchanges.what;
using Arbitrage.Test.CoinNames;
using System.Text.Json;

namespace Arbitrage.Symbols.CoinNames.Exchanges
{
    public class Lbank : IMarketPairApi
    {
        private record ApiConfig(
            string Url,
            Func<JsonElement, MarketPair> Parser,
            Func<string, bool>? Filter = null
        );

        private static readonly Dictionary<MarketType, ApiConfig> Configs = new()
        {
            [MarketType.Spot] = new(
                Url: "https://api.lbkex.com/v2/currencyPairs.do",
                Parser: ParseSpotItem,
                Filter: IsSpotPair
            ),
            [MarketType.Futures] = new(
                Url: "https://lbkperp.lbank.com/cfd/openApi/v1/pub/instrument?productGroup=SwapU",
                Parser: ParseFuturesItem
            )
        };

        public async Task<List<MarketPair>> GetPairs(MarketType marketType)
        {
            if (!Configs.TryGetValue(marketType, out var config))
                throw new ArgumentException("Unsupported market type");

            var response = await Network.GetAsync(config.Url);
            var result = new List<MarketPair>();

            using var doc = JsonDocument.Parse(response);
            var data = doc.RootElement.GetProperty("data");

            foreach (var item in data.EnumerateArray())
            {
                var pair = config.Parser(item);

                if (config.Filter != null && !config.Filter(pair.Ticker))
                    continue;

                pair.Exchange = ExchangeDomain.Exchanges.LBank;
                pair.MarketType = marketType;
                pair.Original = AliasHelper.GetOriginalName(pair);

                result.Add(pair);
            }

            return result;
        }

        // Фильтр спотовых пар (исключает 3l, 3s, 5l, 5s)
        private static bool IsSpotPair(string ticker)
        {
            return !(ticker.Contains("3l") || ticker.Contains("3s")
                 || ticker.Contains("5l") || ticker.Contains("5s"));
        }

        private static MarketPair ParseSpotItem(JsonElement item)
        {
            var ticker = item.GetString();
            var parts = ticker.Split('_');

            return new MarketPair
            {
                Ticker = ticker.ToLower(),
                Base = parts[0].ToUpper(),
                Quote = parts[1].ToUpper()
            };
        }

        private static MarketPair ParseFuturesItem(JsonElement item)
        {
            return new MarketPair
            {
                Ticker = item.GetProperty("symbol").GetString(),
                Base = item.GetProperty("baseCurrency").GetString(),
                Quote = item.GetProperty("clearCurrency").GetString(),
            };
        }
    }
}