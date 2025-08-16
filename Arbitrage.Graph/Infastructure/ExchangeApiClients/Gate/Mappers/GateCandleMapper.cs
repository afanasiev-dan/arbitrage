using System.Globalization;
using Arbitrage.Graph.Domain;
using Arbitrage.Graph.Infastructure.ExchangeApiClients.Gate.Dto;

namespace Arbitrage.Graph.Infastructure.ExchangeApiClients.Gate.Mappers
{
    public static class GateCandleMapper
    {
        public static Candle ToDomainEntity(
        this GateCandleResponceDto dto,
        Guid exchangeId,
        Guid symbolId,
        int interval)
        {
            return new Candle
            {
                ExchangeId = exchangeId,
                CurrencyPairId = symbolId,
                Interval = interval,
                OpenTime = DateTimeOffset.FromUnixTimeSeconds(dto.OpenTime).UtcDateTime,
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