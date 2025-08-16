using System.Text.Json;
using Arbitrage.Graph.Infastructure.ExchangeApiClients.HTX.Dto;

namespace Arbitrage.Graph.Infastructure.ExchangeApiClients.HTX
{
    public class HtxCandleDtoConverter
    {
        
        public static List<HtxCandleResponceDto> Convert(string json)
        {
            using JsonDocument doc = JsonDocument.Parse(json);
            JsonElement root = doc.RootElement;

            // Проверяем структуру ответа HTX (Huobi)
            if (!root.TryGetProperty("data", out var dataElement) && 
                dataElement.ValueKind != JsonValueKind.Array)
            {
                throw new Exception("Invalid response structure");
            }

            var candles = new List<HtxCandleResponceDto>();
            
            foreach (JsonElement candleElement in dataElement.EnumerateArray())
            {
                try
                {
                    var candle = new HtxCandleResponceDto
                    {
                        OpenTime = candleElement.GetProperty("id").GetInt64(),
                        Open = candleElement.GetProperty("open").GetDecimal(),
                        Close = candleElement.GetProperty("close").GetDecimal(),
                        High = candleElement.GetProperty("high").GetDecimal(),
                        Low = candleElement.GetProperty("low").GetDecimal(),
                        Amount = candleElement.GetProperty("amount").GetDecimal(),
                        Volume = candleElement.GetProperty("vol").GetDecimal(),
                    };

                    candles.Add(candle);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error parsing candle: {ex.Message}");
                }
            }
            
            return candles;
        }
    }
}