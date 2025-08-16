using Arbitrage.Other;
using System.Diagnostics;

namespace Arbitrage.Core.Calclucation
{
    public class MathCups
    {
        /// <summary>
        /// Вычисление курсового спреда между двумя биржами в %
        /// </summary>
        /// <param name="longValue"></param>
        /// <param name="shortValue"></param>
        /// <returns></returns>
        public static decimal ExchangeRateSpread(decimal longValue, decimal shortValue)
        {
            decimal result = Math.Round((shortValue / longValue - 1) * 100, 2);
            return result;
        }

        /// <summary>
        /// Расчет средней цены входа в позицию
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        public static decimal AvgPricePos(decimal[] values)
        {
            decimal result = 0;
            for (int i = 0; i < values.Length; i++)
            {
                result = values[i] + result;
            }

            result = result / values.Length;
            return result;
        }
        /// <summary>
        /// Вычисление общего суммы значений
        /// </summary>
        /// <param name="volume"></param>
        /// <returns></returns>
        public static decimal TotalSum(decimal[] volume)
        {
            decimal result = 0;
            for (int i = 0; i < volume.Length; i++)
            {
                result += volume[i];
            }
            return result;
        }

        public static decimal GetSpreadSMA(
            Dictionary<DateTime, decimal> longs,
            Dictionary<DateTime, decimal> shorts,
            int period)
        {
            var commonDates = shorts.Keys.Intersect(longs.Keys).OrderBy(d => d).ToList();

            if (commonDates.Count == 0)
                return decimal.MinValue;

            var lastDates = commonDates.TakeLast(period);
            var spreadValues = lastDates
                .Select(date => (Date: date, Spread: ExchangeRateSpread(longs[date], shorts[date])))
                .ToList();

            var x2 = Math.Round(spreadValues.Average(x => x.Spread), 2);
            return Math.Round(spreadValues.Average(x => x.Spread), 2);
        }

        public static decimal GetProfitFund(DateTime? date1, decimal? value1, DateTime? date2, decimal? value2)
        {
            bool has1 = value1.HasValue && date1.HasValue;
            bool has2 = value2.HasValue && date2.HasValue;

            if (has1 && has2)
            {
                if (F.CompareDateTime(date1.Value, date2.Value))
                    return -value1.Value + value2.Value;
                else if (date1 < date2)
                    return -value1.Value;
                else
                    return value2.Value;
            }

            if (has1) return -value1.Value;
            if (has2) return value2.Value;

            return 0;
        }

    }
}
