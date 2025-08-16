using System.Text.Json.Serialization;

namespace Arbitrage.Graph.Infastructure.ExchangeApiClients.Mexc.Dto
{
    public class MexcFuturesResponse
    {
        [JsonPropertyName("success")]
        public bool Success { get; set; }
        
        [JsonPropertyName("code")]
        public int Code { get; set; }
        
        [JsonPropertyName("data")]
        public FuturesCandleData? Data { get; set; }
    }
}