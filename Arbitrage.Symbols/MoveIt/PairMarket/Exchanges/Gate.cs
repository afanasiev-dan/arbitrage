using Arbitrage.ExchangeDomain;
using Arbitrage.ExchangeDomain.Enums;
using Arbitrage.Symbols.CoinNames.Alias;
using Arbitrage.Symbols.PairNames.Exchanges.what;
using Arbitrage.Test.CoinNames;
using System.Text.Json;

namespace Arbitrage.Symbols.CoinNames.Exchanges
{
    public class Gate : IMarketPairApi
    {
        private record ApiConfig(
            string Url,
            string TickerField
        );

        private static readonly Dictionary<MarketType, ApiConfig> Configs = new()
        {
            [MarketType.Spot] = new(
                Url: "https://api.gateio.ws/api/v4/spot/tickers",
                TickerField: "currency_pair"
            ),
            [MarketType.Futures] = new(
                Url: "https://api.gateio.ws/api/v4/futures/usdt/contracts",
                TickerField: "name"
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

            foreach (var item in root.EnumerateArray())
            {
                string ticker = item.GetProperty(config.TickerField).GetString();

                var pair = new MarketPair
                {
                    Ticker = ticker,
                    Base = ticker.Split('_')[0],
                    Quote = ticker.Split('_')[1],
                    Exchange = ExchangeDomain.Exchanges.Gate,
                    MarketType = marketType,
                    Original = AliasHelper.GetOriginalName(new MarketPair
                    {
                        Base = ticker.Split('_')[0],
                        Quote = ticker.Split('_')[1]
                    })
                };

                result.Add(pair);
            }

            return result;
        }
    }
}