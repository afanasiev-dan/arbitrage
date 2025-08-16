using Arbitrage.ExchangeDomain;
using Arbitrage.ExchangeDomain.Enums;
using Arbitrage.Test.CoinNames;

namespace Arbitrage.Symbols.CoinNames.Alias
{
    internal class AliasHelper
    {
        static readonly Dictionary<string, AliasGroup> coinAliasMap = new Dictionary<string, AliasGroup>
        {
            {
                "XBT", new AliasGroup
                {
                    Original = "BTC",
                    Mappings =
                    [
                        new ExchangeMapping { Exchange = ExchangeDomain.Exchanges.KuCoin, TypeMarket = MarketType.Futures }
                    ]
                }
            },
        };

        public static string GetOriginalName(MarketPair pair)
        {
            if (coinAliasMap.TryGetValue(pair.Base.ToUpper(), out var aliasGroup))
            {
                var hasMapping = aliasGroup.Mappings.Any(x =>
                    x.Exchange == pair.Exchange &&
                    x.TypeMarket == pair.MarketType);

                if (hasMapping)
                {
                    return aliasGroup.Original;
                }
            }
            return pair.Base.ToUpper();
        }

        public class AliasGroup
        {
            public string Original { get; set; }
            public List<ExchangeMapping> Mappings { get; set; }
        }

        public class ExchangeMapping
        {
            public string Exchange { get; set; }
            public MarketType TypeMarket { get; set; }
        }
    }
}