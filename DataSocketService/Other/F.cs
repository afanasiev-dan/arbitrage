using Arbitrage.Domain;
using Arbitrage.ExchangeDomain.Enums;
using Arbitrage.Symbols.Presentation.Dto.CurrencyPair;
using DataSocketService.Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataSocketService.Other
{
    internal class F
    {
        public static decimal ToDec(string value)
        {
            value = value.Replace(" ", "").Replace(",", ".");
            return decimal.Parse(value,
            NumberStyles.Float | NumberStyles.AllowExponent,
            CultureInfo.InvariantCulture);
        }

        public static string ExchangeToStr(string exchangeName, MarketType marketType)
            => $"{exchangeName}-{marketType}";
        public static async Task<List<CurrencyPairResponceDto>> GetCurrencyPair()
        {
            //var response = await Network.GetAsync("https://localhost:7102/CurrencyPair/currency-pairs");
            var response = File.ReadAllText("data/pairs.json");
            var apiResponse = JsonConvert.DeserializeObject<ApiResponce>(response);
            if (apiResponse.Result is JArray jArray)
                return jArray.ToObject<List<CurrencyPairResponceDto>>();
            return null;
        }

        public static List<ArbitragePair> GetArbitragePair(List<CurrencyPairResponceDto> currencyPair)
        {
            var groupedPairs = currencyPair
             .GroupBy(p => new { p.BaseCoin, p.QuoteCoin })
             .Where(g => g.Count() >= 2);

            var arbPairs = new List<ArbitragePair>();

            foreach (var group in groupedPairs)
            {
                var futures = group.Where(p => p.MarketType == MarketType.Futures).ToList();
                var spots = group.Where(p => p.MarketType == MarketType.Spot).ToList();

                // Spot + futures
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
}
