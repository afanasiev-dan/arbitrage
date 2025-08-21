using Arbitrage.Domain;
using Arbitrage.ExchangeDomain.Enums;
using Arbitrage.Symbols.Presentation.Dto.CurrencyPair;
using DataSocketService.Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DataSocketService.CurrencyPairF;

public static class PairF
{
    public static async Task<List<CurrencyPairResponceDto>> GetCurrency()
    {
        // TODO: вынести источник в конфиг (сейчас локальный JSON)
        var response = File.ReadAllText("data/pairs.json");
        var apiResponse = JsonConvert.DeserializeObject<ApiResponce>(response);

        if (apiResponse?.Result is JArray jArray)
            return jArray.ToObject<List<CurrencyPairResponceDto>>();

        return new List<CurrencyPairResponceDto>();
    }

    public static List<ArbitragePair> GetArbitrage(List<CurrencyPairResponceDto> currencyPairs)
    {
        var groupedPairs = currencyPairs
            .GroupBy(p => new { p.BaseCoin, p.QuoteCoin })
            .Where(g => g.Count() >= 2);

        var arbPairs = new List<ArbitragePair>();

        foreach (var group in groupedPairs)
        {
            var futures = group.Where(p => p.MarketType == MarketType.Futures).ToList();
            var spots = group.Where(p => p.MarketType == MarketType.Spot).ToList();

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
