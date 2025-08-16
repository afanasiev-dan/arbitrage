using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Arbitrage.ExchangeDomain;
using Arbitrage.Graph.Domain;
using Arbitrage.Graph.Infastructure.ExchangeApiClients.Bybit.Dto;

namespace Arbitrage.Graph.Infastructure.ExchangeApiClients.Bybit.Mappers
{
    public static class BybitCandleMapper
    {
        public static Candle ToDomainEntity(
        this BybitCandleResponceDto dto,
        Guid exchangeId,
        Guid symbolId,
        int interval)
        {
            return new Candle
            {
                ExchangeId = exchangeId,
                CurrencyPairId = symbolId,
                Interval = interval,
                OpenTime = DateTimeOffset.FromUnixTimeMilliseconds(dto.OpenTime).UtcDateTime,
                Open = ParseDecimal(dto.Open),
                High = ParseDecimal(dto.High),
                Low = ParseDecimal(dto.Low),
                Close = ParseDecimal(dto.Close),
                Volume = ParseDecimal(dto.Volume),
            };
        }

        private static decimal ParseDecimal(string value)
        {
            return decimal.Parse(value, NumberStyles.Float, CultureInfo.InvariantCulture);
        }


    }
}