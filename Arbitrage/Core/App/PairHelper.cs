using Arbitrage.Core.Base.Enums;
using Arbitrage.Core.Calclucation;
using Arbitrage.Service.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arbitrage.Core.App
{
    public class PairHelper
    {
        public static void FilterPair((Exchange Exchange, List<CoinInfo> InfoCryptos)[] r)
        {
            var coinPresence = new Dictionary<string, List<(ExchangeEnum ExchangeName, AssetTypeEnum Type)>>();

            foreach (var (exchange, coins) in r)
            {
                foreach (var coin in coins)
                {
                    if (string.IsNullOrEmpty(coin.BaseCoin))
                        continue;

                    var baseCoin = coin.BaseCoin;

                    if (!coinPresence.ContainsKey(baseCoin))
                        coinPresence[baseCoin] = new List<(ExchangeEnum, AssetTypeEnum)>();

                    coinPresence[baseCoin].Add((exchange.Settings.Name, exchange.Settings.AssetType));
                }
            }

            // Фильтруем монеты, оставляем только те, где есть:
            // - хотя бы одна биржа с этим коином на фьючерсе (для шорта)
            // - и хотя бы одна биржа с этим коином на споте или фьючерсе (для лонга)
            var arbitrageableCoins = coinPresence
                .Where(kvp =>
                {
                    var entries = kvp.Value;

                    // Находим все биржи, где есть этот актив на фьючерсе
                    var shortExchanges = entries
                        .Where(e => e.Type == AssetTypeEnum.Futures)
                        .Select(e => e.ExchangeName)
                        .Distinct()
                        .ToList();

                    // Находим все биржи, где есть этот актив на лонг (Spot или Futures)
                    var longEntries = entries
                        .Where(e => e.Type == AssetTypeEnum.Spot || e.Type == AssetTypeEnum.Futures)
                        .ToList();

                    // Проверим, можно ли найти хотя бы одну пару бирж (лонг ≠ шорт)
                    foreach (var longEntry in longEntries)
                    {
                        foreach (var shortExchange in shortExchanges)
                        {
                            if (longEntry.ExchangeName != shortExchange ||
                                longEntry.Type != AssetTypeEnum.Futures) // Не использовать один и тот же фьючерс для лонга и шорта
                            {
                                return true;
                            }
                        }
                    }

                    return false;
                })
                .Select(kvp => kvp.Key)
                .ToHashSet();


            // Применяем фильтр
            for (int i = 0; i < r.Length; i++)
            {
                r[i].InfoCryptos = r[i].InfoCryptos
                    .Where(c => arbitrageableCoins.Contains(c.BaseCoin))
                    .ToList();
            }
        }

        public static List<ResultPair> CreatePairs(List<Crypto> cryptos)
        {
            var groupedCryptos = cryptos
                .GroupBy(c => c.Info.Coin.BaseCoin)
                .Select(g => g.ToList())
                .ToList();
            groupedCryptos = groupedCryptos.Where(x => x.Count >= 2).ToList();

            var results = new List<ResultPair>();
            foreach (var group in groupedCryptos)
            {
                foreach (var longPos in group)
                {
                    foreach (var shortPos in group)
                    {
                        if (longPos != shortPos && shortPos.Info.AssetType != AssetTypeEnum.Spot)
                        {
                            var resultPair = new ResultPair
                            {
                                LongPos = longPos,
                                ShortPos = shortPos
                            };

                            bool add = true;
                            if (LaunchConfig.SMAEnable)
                            {
                                resultPair.CalculateSMA();
                                add = resultPair.SMA > LaunchConfig.limit_low && resultPair.SMA < LaunchConfig.limit_up;
                            }

                            if (add)
                                results.Add(resultPair);
                        }
                    }
                }
            }
            return results;
        }
        public static void UpdatePairs(List<ResultPair> pairs, decimal volume)
        {
            if (!LaunchConfig.PriceEnable) return;

            foreach (var pair in pairs)
            {
                if (pair.LongPos.SuccesBook() && pair.ShortPos.SuccesBook())
                {
                    pair.CalculateSpred(volume);
                    bool success_spread = pair.Spread > LaunchConfig.limit_low && pair.Spread < LaunchConfig.limit_up;
                    pair.HasError = !success_spread;

                }
                else
                {
                    pair.HasError = true;
                    //pair.SuccessDT = null;
                }
            }
        }
    }
}
