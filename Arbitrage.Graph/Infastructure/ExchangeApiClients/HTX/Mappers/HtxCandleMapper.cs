using System.Globalization;
using Arbitrage.ExchangeDomain;
using Arbitrage.Graph.Domain;
using Arbitrage.Graph.Infastructure.ExchangeApiClients.HTX.Dto;

namespace Arbitrage.Graph.Infastructure.ExchangeApiClients.HTX.Mappers
{
    public static class HtxCandleMapper
    {
        public static Candle ToDomainEntity(
            this HtxCandleResponceDto dto,
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
                Open = dto.Open,
                High = dto.High,
                Low = dto.Low,
                Close = dto.Close,
                Volume = dto.Volume,
            };
        }

        private static decimal ParseDecimal(string value)
        {
            return decimal.Parse(value, NumberStyles.Float, CultureInfo.InvariantCulture);
        }
    }
}