using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arbitrage.Scaner.Utils
{
    public class ArbitrageCalculator
    {
        public static decimal GetArbitrageRate(decimal longPrice, decimal shortPrice, bool isRound = true)
        {
            if (longPrice == 0)
                return -999;
            if (shortPrice == 0)
                return -999;
            var rate = ((shortPrice / longPrice) - 1) * 100;
            return isRound? Math.Round(rate, 2) : rate;
        }
    }
}
