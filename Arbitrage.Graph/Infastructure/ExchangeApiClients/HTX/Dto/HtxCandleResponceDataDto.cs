using System.Text.Json.Serialization;

namespace Arbitrage.Graph.Infastructure.ExchangeApiClients.HTX.Dto
{
    public class HtxCandleResponceDataDto
    {
        [JsonPropertyName("ch")]
        public string Channel { get; set; }

        [JsonPropertyName("status")]
        public string Status { get; set; }

        [JsonPropertyName("ts")]
        public long Timestamp { get; set; }

        [JsonPropertyName("data")]
        public List<HtxCandleResponceDto> Data { get; set; }

        public DateTimeOffset GetResponseTime() => DateTimeOffset.FromUnixTimeMilliseconds(Timestamp);
    }
}