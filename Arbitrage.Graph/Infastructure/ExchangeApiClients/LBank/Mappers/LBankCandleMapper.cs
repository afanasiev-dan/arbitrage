using System.Globalization;
using Arbitrage.ExchangeDomain;using Arbitrage.Graph.Domain;
using Arbitrage.Graph.Infastructure.ExchangeApiClients.LBank.Dto;

namespace Arbitrage.Graph.Infastructure.ExchangeApiClients.LBank.Mappers
{
    public static class LBankCandleMapper
    {
        public static Candle ToDomainEntity(
            this LBankCandleResponceDto dto,
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
            value = value.Replace(",", ".");
            return decimal.Parse(value, NumberStyles.Float, CultureInfo.InvariantCulture);
        }

    }
}