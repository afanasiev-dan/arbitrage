using System.Text.Json.Serialization;

namespace Arbitrage.Graph.Infastructure.ExchangeApiClients.Mexc.Dto
{
    public class FuturesCandleData
    {
        [JsonPropertyName("time")]
        public long[] Time { get; set; } = Array.Empty<long>();
        
        [JsonPropertyName("open")]
        public double[] Open { get; set; } = Array.Empty<double>();
        
        [JsonPropertyName("close")]
        public double[] Close { get; set; } = Array.Empty<double>();
        
        [JsonPropertyName("high")]
        public double[] High { get; set; } = Array.Empty<double>();
        
        [JsonPropertyName("low")]
        public double[] Low { get; set; } = Array.Empty<double>();
        
        [JsonPropertyName("vol")]
        public double[] Vol { get; set; } = Array.Empty<double>();
        
        [JsonPropertyName("amount")]
        public decimal[] Amount { get; set; } = Array.Empty<decimal>();
    }
}