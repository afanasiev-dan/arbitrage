using System.Text.Json.Serialization;

namespace Arbitrage.Graph.Infastructure.ExchangeApiClients.KuCoin.Dto
{
    public class KuCoinApiSpotResponseDto
    {
        [JsonPropertyName("code")]
        public string Code { get; set; }

        [JsonPropertyName("data")]
        public List<string[]> Candles { get; set; }
    }

    public class KuCoinApiFutureResponseDto
    {
        [JsonPropertyName("code")]
        public string Code { get; set; }

        [JsonPropertyName("data")]
        public List<decimal[]> Candles { get; set; }
    }
}