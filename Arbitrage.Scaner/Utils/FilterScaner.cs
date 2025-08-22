using Arbitrage.ExchangeDomain.Enums;
using Arbitrage.Scaner.Domain.Entities;
using Arbitrage.Scaner.Presentation.Dto;

namespace Arbitrage.Scaner.Utils
{
    internal class FilterScaner
    {
        public static List<ScanerModel> ApplyFilter(IEnumerable<ScanerModel> result, FilterRequestDto filter)
        {
            var filteredResult = new List<ScanerModel>();

            foreach (var s in result)
            {
                bool include = true;
                if (filter.SpotExchanges != null && s.MarketTypeLong == MarketType.Spot)
                {
                    if (!filter.SpotExchanges.Contains(s.ExchangeLong.Name))
                    {
                        include = false;
                    }
                }
                if (include && filter.FuturesExchanges != null)
                {
                    if (s.MarketTypeShort == MarketType.Futures && !filter.FuturesExchanges.Contains(s.ExchangeShort.Name))
                    {
                        include = false;
                    }

                    if (include && s.MarketTypeLong == MarketType.Futures && !filter.FuturesExchanges.Contains(s.ExchangeLong.Name))
                    {
                        include = false;
                    }
                }

                if (filter.MinArbitrageRate != null)
                {
                    var arbitrageRate = ArbitrageCalculator.GetArbitrageRate(s.PurchasePriceLong, s.PurchasePriceShort);
                    if (arbitrageRate < filter.MinArbitrageRate)
                        include = false;
                }

                if (include)
                {
                    filteredResult.Add(s);
                }
            }

            return filteredResult;
        }
    }
}
