using System.Globalization;
using Arbitrage.Exchange.Infastructure.Repositories;
using Arbitrage.Graph.Domain;
using Arbitrage.Graph.Infastructure.ExchangeApiClients.Mexc.Dto;
using Arbitrage.Symbols.Infastructure.Repositories;

namespace Arbitrage.Graph.Infastructure.ExchangeApiClients.Mexc.Mappers
{
    public static class MexcCandleMapper
    {
        public static Candle ToDomainEntity(
            this MexcCandleResponceDto dto,
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