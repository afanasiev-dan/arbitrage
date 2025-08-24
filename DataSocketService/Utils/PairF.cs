using Arbitrage.Domain;
using Arbitrage.ExchangeDomain;
using Arbitrage.ExchangeDomain.Enums;
using Arbitrage.Symbols.Presentation.Dto.CurrencyPair;
using DataSocketService.Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DataSocketService.Utils;

public static class PairF
{
    public static async Task<List<CurrencyPairResponceDto>> GetCurrency()
    {
        var response = await Network.GetAsync("https://localhost:7102/CurrencyPair/currency-pairs");
        var apiResponse = JsonConvert.DeserializeObject<ApiResponce>(response);
        if (apiResponse?.Result is JArray jArray)
        {
            var lst = jArray.ToObject<List<CurrencyPairResponceDto>>();
            lst = lst.Where(x =>
            x.ExchangeName == Arbitrage.ExchangeDomain.Exchanges.Gate
            || (x.ExchangeName == Arbitrage.ExchangeDomain.Exchanges.Mexc && x.MarketType == MarketType.Futures)
            || x.ExchangeName == Arbitrage.ExchangeDomain.Exchanges.ByBit
            || x.ExchangeName == Arbitrage.ExchangeDomain.Exchanges.KuCoin
            || (x.ExchangeName == Arbitrage.ExchangeDomain.Exchanges.LBank && x.MarketType == MarketType.Spot)
            || x.ExchangeName == Arbitrage.ExchangeDomain.Exchanges.Htx
            ).ToList();

            //lst = lst.Where(x => x.BaseCoin == "XMR" &&
            //!(x.ExchangeName == Arbitrage.ExchangeDomain.Exchanges.LBank && x.MarketType == MarketType.Futures) &&
            //!(x.ExchangeName == Arbitrage.ExchangeDomain.Exchanges.Mexc && x.MarketType == MarketType.Spot) &&
            //!(x.ExchangeName == Arbitrage.ExchangeDomain.Exchanges.Binance)
            //).ToList();
            return lst;
        }
        return null;

        //string msg = File.ReadAllText("data/pairs.json");
        //var t = JsonConvert.DeserializeObject<List<CurrencyPairResponceDto>>(msg);
        //t = t.Where(x => x.ExchangeName == Arbitrage.ExchangeDomain.Exchanges.ByBit).ToList();// && x.MarketType == MarketType.Futures
        //return t;
    }

    public static List<ArbitragePair> GetArbitrage(List<CurrencyPairBook> currencyPairs)
    {
        var groupedPairs = currencyPairs
            .GroupBy(p => new { p.Info.BaseCoin, p.Info.QuoteCoin })
            .Where(g => g.Count() >= 2);

        var arbPairs = new List<ArbitragePair>();

        foreach (var group in groupedPairs)
        {
            var futures = group.Where(p => p.Info.MarketType == MarketType.Futures).ToList();
            var spots = group.Where(p => p.Info.MarketType == MarketType.Spot).ToList();

            // Spot + Futures
            if (spots.Any() && futures.Any())
            {
                foreach (var spot in spots)
                    foreach (var future in futures)
                        arbPairs.Add(new ArbitragePair { LongPair = spot, ShortPair = future });
            }

            // Futures + Futures (в обе стороны)
            if (futures.Count >= 2)
            {
                for (int i = 0; i < futures.Count; i++)
                    for (int j = i + 1; j < futures.Count; j++)
                    {
                        arbPairs.Add(new ArbitragePair { LongPair = futures[i], ShortPair = futures[j] });
                        arbPairs.Add(new ArbitragePair { LongPair = futures[j], ShortPair = futures[i] });
                    }
            }
        }

        return arbPairs;
    }
}
