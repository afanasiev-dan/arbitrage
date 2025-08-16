using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arbitrage.Core
{
    public class G
    {
        static Dictionary<string,string> coinAliasMap = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "XBT", "BTC" },
            { "ETH2", "ETH" },
            { "USDTM", "USDT" },
        };
        public static string Map(string value) =>
            coinAliasMap.TryGetValue(value, out var mapped) ? mapped : value;
    }
}
