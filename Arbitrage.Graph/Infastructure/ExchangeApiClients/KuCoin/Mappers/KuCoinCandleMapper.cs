using System.Globalization;
using Arbitrage.Graph.Domain;
using Arbitrage.Graph.Infastructure.ExchangeApiClients.KuCoin.Dto;

namespace Arbitrage.Graph.Infastructure.ExchangeApiClients.KuCoin.Mappers
{
    public static class KuCoinCandleMapper
    {
        public static Candle SpotToDomainEntity(
            this KuCoinCandleResponceDto dto,
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

         public static Candle FutureToDomainEntity(
            this KuCoinCandleResponceDto dto,
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
            value = value.Replace(",", ".");
            return decimal.Parse(value, NumberStyles.Float, CultureInfo.InvariantCulture);
        }
    }
}