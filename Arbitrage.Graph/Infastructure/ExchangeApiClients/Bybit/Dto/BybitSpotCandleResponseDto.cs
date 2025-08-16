using System.Text.Json.Serialization;

namespace Arbitrage.Graph.Infastructure.ExchangeApiClients.Bybit.Dto
{
    public class BybitSpotCandleResponseDto
    {

        [JsonPropertyName("retCode")]
        public int RetCode { get; set; }

        [JsonPropertyName("retMsg")]
        public string RetMsg { get; set; }

        [JsonPropertyName("result")]
        public BybitSpotCandleResultDto Result { get; set; }

        [JsonPropertyName("retExtInfo")]
        public Dictionary<string, object> RetExtInfo { get; set; }

        [JsonPropertyName("time")]
        public long Time { get; set; }
    }
}