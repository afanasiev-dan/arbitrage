using System.Text.Json.Serialization;

namespace Arbitrage.Graph.Infastructure.ExchangeApiClients.HTX.Dto
{
    public class HtxCandleResponceDto
    {
        [JsonPropertyName("id")]
        public long OpenTime { get; set; }  // Timestamp в секундах

        [JsonPropertyName("open")]
        public decimal Open { get; set; }

        [JsonPropertyName("close")]
        public decimal Close { get; set; }

        [JsonPropertyName("low")]
        public decimal Low { get; set; }

        [JsonPropertyName("high")]
        public decimal High { get; set; }

        [JsonPropertyName("amount")]
        public decimal Amount { get; set; }  // Объем в базовой валюте (BTC)

        [JsonPropertyName("vol")]
        public decimal Volume { get; set; }  // Объем в котируемой валюте (USDT)

        [JsonPropertyName("count")]
        public int TradeCount { get; set; }

        public DateTimeOffset GetOpenTime() => DateTimeOffset.FromUnixTimeSeconds(OpenTime);
    }
}