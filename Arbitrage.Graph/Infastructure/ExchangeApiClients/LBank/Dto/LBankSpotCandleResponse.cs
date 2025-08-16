using System.Text.Json.Serialization;

namespace Arbitrage.Graph.Infastructure.ExchangeApiClients.LBank.Dto
{
    public class LBankSpotDataResponseDto
    {
        [JsonPropertyName("msg")]
        public string Message { get; set; }

        [JsonPropertyName("result")]
        public string Result { get; set; }

        [JsonPropertyName("data")]
        public List<List<decimal>> Data { get; set; }

        [JsonPropertyName("error_code")]
        public int ErrorCode { get; set; }

        [JsonPropertyName("ts")]
        public long Timestamp { get; set; }
    }
}