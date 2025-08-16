using Arbitrage.ExchangeDomain.Enums;
using Arbitrage.Test.CoinNames;

namespace Arbitrage.Symbols.MoveIt.PairArbitrage
{
    internal class ArbitragePairHelper
    {
        public static List<ArbPair> Create(List<MarketPair> marketPairs)
        {
            var groupedPairs = marketPairs
                .GroupBy(p => new { p.Original, p.Quote })
                .Where(g => g.Count() >= 2);

            var arbPairs = new List<ArbPair>();

            foreach (var group in groupedPairs)
            {
                var futures = group.Where(p => p.MarketType == MarketType.Futures).ToList();
                var spots = group.Where(p => p.MarketType == MarketType.Spot).ToList();

                // Spot + futures
                if (spots.Any() && futures.Any())
                {
                    foreach (var spot in spots)
                        foreach (var future in futures)
                            arbPairs.Add(new ArbPair { LongPair = spot, ShortPair = future });
                }

                // Futures + Futures (в обе стороны)
                if (futures.Count >= 2)
                {
                    for (int i = 0; i < futures.Count; i++)
                        for (int j = i + 1; j < futures.Count; j++)
                        {
                            arbPairs.Add(new ArbPair { LongPair = futures[i], ShortPair = futures[j] });
                            arbPairs.Add(new ArbPair { LongPair = futures[j], ShortPair = futures[i] });
                        }
                }
            }

            return arbPairs;
        }
    }
}
