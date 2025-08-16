using Arbitrage.Service.Base;

namespace Arbitrage.Core.Calclucation
{
    public class CupsCalculation
    {
        /// <summary>
        /// Расчет курсового спреда между двумя биржами, при входе в несколько ордеров
        /// </summary>
        /// <param name="longValues"></param>
        /// <param name="shortValues"></param>
        /// <returns></returns>
        public static decimal SpredCalculationBetweenTwoExchanges(decimal[] longValues, decimal[] shortValues)
        {
            decimal shortAvg = MathCups.AvgPricePos(shortValues);
            decimal longAvg = MathCups.AvgPricePos(longValues);
            decimal result = MathCups.ExchangeRateSpread(longAvg, shortAvg);
            return result;
        }

        /// <summary>
        /// Расчёт курсового спреда между двумя биржами с объёмами
        /// </summary>
        /// <param name="volume">Общий объем</param>
        /// <param name="avgPrice">Средняя цена входа</param>
        /// <returns></returns>
        public static (decimal, decimal) CalculatingTotalVolumeAndAvgPrice(decimal[] volume, decimal[] avgPrice)
        {
            if (volume.Length != avgPrice.Length)
                throw new Exception("Количество значений объёма и цены входа должны совпадать");

            decimal total_volume = MathCups.TotalSum(volume);
            decimal avg_price = MathCups.AvgPricePos(avgPrice);

            return (total_volume, avg_price);
        }

        public static (bool success, decimal price) CalculateAverageEntryPrice(List<(decimal price, decimal volume)> book, decimal targetAmountInUSD, bool print = false)
        {
            decimal totalCoins = 0;
            decimal totalSpentUSD = 0;
            decimal remainingUSD = targetAmountInUSD;

            foreach (var ask in book)
            {
                decimal price = ask.price;
                decimal coinVolume = ask.volume;
                decimal availableUSD = coinVolume * price;

                if (availableUSD >= remainingUSD)
                {
                    // Достаточно на этом уровне
                    decimal coinsToBuy = remainingUSD / price;
                    totalCoins += coinsToBuy;
                    totalSpentUSD += coinsToBuy * price;
                    remainingUSD = 0;
                    break;
                }
                else
                {
                    // Покупаем всё на этом уровне
                    totalCoins += coinVolume;
                    totalSpentUSD += availableUSD;
                    remainingUSD -= availableUSD;
                }
            }

            decimal averagePrice = totalSpentUSD / totalCoins;
            decimal minPrice = book.First().price;
            int decimals = GetDecimalPlaces(minPrice);

            //if (print)
            //    Console.WriteLine($"Coins={totalCoins} Spent={totalSpentUSD} Осталось={remainingUSD} Цель={targetAmountInUSD}");

            if (remainingUSD > 0)
            {
                return (false, remainingUSD);
            }
            else
                return (true, Math.Round(averagePrice, decimals));
        }
        private static int GetDecimalPlaces(decimal number)
        {
            string str = number.ToString("0.####################");
            int index = str.IndexOf(',');
            return index < 0 ? 0 : str.Length - index - 1;
        }
    }
}



// 10.1 - 100 монет
// 10.2 - 150 монет
// 10.3 - 200 монет

// ср.ц - общ монеты - $
// 10.2 - 450 - 44,11