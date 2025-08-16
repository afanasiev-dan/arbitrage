using System.Text.Json.Serialization;

namespace Arbitrage.Graph.Infastructure.ExchangeApiClients.Bybit.Dto
{
    public class BybitSpotCandleResultDto
    {
        [JsonPropertyName("category")]
        public string Category { get; set; }

        [JsonPropertyName("symbol")]
        public string Symbol { get; set; }

        [JsonPropertyName("list")]
        public List<string[]> Candles { get; set; } // Массив строковых значений свечи
    }
}