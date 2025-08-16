using Arbitrage.ExchangeDomain;
using Arbitrage.ExchangeDomain.Enums;
using Arbitrage.Symbols.PairNames.Exchanges.what;
using Arbitrage.Test.CoinNames;
using Newtonsoft.Json;

namespace Arbitrage.Symbols
{
    public class MarketPairUpdater
    {
        static readonly List<(string exchange, MarketType typeExchange)> exchangeAssets = new()
        {
            (Exchanges.ByBit, MarketType.Spot),
            (Exchanges.ByBit, MarketType.Futures),
            (Exchanges.KuCoin, MarketType.Spot),
            (Exchanges.KuCoin, MarketType.Futures),
            (Exchanges.Gate, MarketType.Spot),
            (Exchanges.Gate, MarketType.Futures),
            (Exchanges.Mexc, MarketType.Spot),
            (Exchanges.Mexc, MarketType.Futures),
            (Exchanges.Htx, MarketType.Spot),
            (Exchanges.Htx, MarketType.Futures),
            (Exchanges.LBank, MarketType.Spot),
            (Exchanges.LBank, MarketType.Futures),
            (Exchanges.Binance, MarketType.Spot),
            (Exchanges.Binance, MarketType.Futures),
        };

        public static async Task Update()
        {
            List<MarketPair> pairs = new();
            var tasks = exchangeAssets.Select(async asset =>
            {
                var api = MarketPairApiFactory.GetExchangeApi(asset.exchange);
                return await api.GetPairs(asset.typeExchange);
            }).ToList();
            var results = await Task.WhenAll(tasks);
            pairs.AddRange(results.SelectMany(x => x));

            string folderPath = "data";
            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);

            var json = JsonConvert.SerializeObject(pairs, Formatting.Indented);
            await File.WriteAllTextAsync($"{folderPath}/marketPairs.json", json);

        }
    }
}
