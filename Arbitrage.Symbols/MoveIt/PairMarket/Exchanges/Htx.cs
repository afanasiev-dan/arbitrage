using Arbitrage.ExchangeDomain;
using Arbitrage.ExchangeDomain.Enums;
using Arbitrage.Symbols.CoinNames.Alias;
using Arbitrage.Symbols.PairNames.Exchanges.what;
using Arbitrage.Test.CoinNames;
using System.Text.Json;

namespace Arbitrage.Symbols.CoinNames.Exchanges
{
    public class Htx : IMarketPairApi
    {
        private record ApiConfig(
            string Url,
            string TickerField,
            string BaseField,
            string QuoteField
        );

        private static readonly Dictionary<MarketType, ApiConfig> Configs = new()
        {
            [MarketType.Spot] = new(
                Url: "https://api.huobi.pro/v1/common/symbols",
                TickerField: "symbol",
                BaseField: "base-currency",
                QuoteField: "quote-currency"
            ),
            [MarketType.Futures] = new(
                Url: "https://api.hbdm.com/linear-swap-api/v1/swap_contract_info",
                TickerField: "contract_code",
                BaseField: "symbol",
                QuoteField: "trade_partition"
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
                var pair = new MarketPair
                {
                    Ticker = item.GetProperty(config.TickerField).GetString(),
                    Base = item.GetProperty(config.BaseField).GetString(),
                    Quote = item.GetProperty(config.QuoteField).GetString()
                };
                pair.Exchange = ExchangeDomain.Exchanges.Htx;
                pair.MarketType = marketType;
                pair.Original = AliasHelper.GetOriginalName(pair);
                result.Add(pair);
            }
            return result;
        }
    }
}
