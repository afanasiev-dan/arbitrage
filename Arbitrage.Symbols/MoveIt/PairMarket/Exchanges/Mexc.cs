using Arbitrage.ExchangeDomain;
using Arbitrage.ExchangeDomain.Enums;
using Arbitrage.Symbols.CoinNames.Alias;
using Arbitrage.Symbols.PairNames.Exchanges.what;
using Arbitrage.Test.CoinNames;
using System.Text.Json;

namespace Arbitrage.Symbols.CoinNames.Exchanges
{
    public class Mexc : IMarketPairApi
    {
        private record ApiConfig(
            string Url,
            string DataPath,
            string BaseField,
            string QuoteField
        );

        private static readonly Dictionary<MarketType, ApiConfig> Configs = new()
        {
            [MarketType.Spot] = new(
                Url: "https://api.mexc.com/api/v3/exchangeInfo",
                DataPath: "symbols",
                BaseField: "baseAsset",
                QuoteField: "quoteAsset"
            ),
            [MarketType.Futures] = new(
                Url: "https://contract.mexc.com/api/v1/contract/detail",
                DataPath: "data",
                BaseField: "baseCoinName",
                QuoteField: "quoteCoin"
            )
        };

        public async Task<List<MarketPair>> GetPairs(MarketType marketType)
        {
            if (!Configs.TryGetValue(marketType, out var config))
                throw new ArgumentException("Unsupported market type");

            var response = await Network.GetAsync(config.Url);
            var result = new List<MarketPair>();

            using var doc = JsonDocument.Parse(response);
            var data = doc.RootElement.GetProperty(config.DataPath);

            foreach (var item in data.EnumerateArray())
            {
                var ticker = item.GetProperty("symbol").GetString();
                var pair = new MarketPair
                {
                    Ticker = ticker,
                    Base = item.GetProperty(config.BaseField).GetString(),
                    Quote = item.GetProperty(config.QuoteField).GetString(),
                    Exchange = ExchangeDomain.Exchanges.Mexc,
                    MarketType = marketType
                    
                };
                pair.Original = AliasHelper.GetOriginalName(pair);
                result.Add(pair);
            }

            return result;
        }
    }
}