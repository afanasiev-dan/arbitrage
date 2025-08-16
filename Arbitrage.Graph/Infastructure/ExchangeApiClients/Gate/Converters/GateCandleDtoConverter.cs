using System.Globalization;
using System.Text.Json;
using Arbitrage.Graph.Infastructure.ExchangeApiClients.Gate.Dto;

namespace Arbitrage.Graph.Infastructure.ExchangeApiClients.Gate
{
    public class GateCandleDtoConverter
    {
        public static List<GateCandleResponceDto> SpotConvert(string json)
        {
            var candles = new List<GateCandleResponceDto>();

            try
            {
                // Десериализуем как массив массивов
                var candleArrays = JsonSerializer.Deserialize<List<List<JsonElement>>>(json);

                if (candleArrays == null)
                    return candles;

                foreach (var candleArray in candleArrays)
                {
                    try
                    {
                        if (candleArray.Count < 8)
                            continue;

                        var candle = new GateCandleResponceDto
                        {
                            OpenTime = int.Parse(candleArray[0].GetString()),
                            QuoteAssetVolume = ParseDecimal(candleArray[1]),
                            Open = candleArray[2].GetString() ?? "",
                            High = candleArray[3].GetString() ?? "",
                            Low = candleArray[4].GetString() ?? "",
                            Close = candleArray[5].GetString() ?? "",
                            Volume = candleArray[6].GetString() ?? "",
                            IsClosed = candleArray[7].GetString() == "true"
                        };

                        candles.Add(candle);
                    }
                    catch (Exception ex)
                    {
                        // Логирование ошибки для конкретной свечи
                        Console.WriteLine($"Error parsing candle: {ex.Message}");
                    }
                }
            }
            catch (Exception ex)
            {
                // Логирование общей ошибки парсинга
                Console.WriteLine($"Error parsing JSON: {ex.Message}");
            }

            return candles;
        }

        public static List<GateCandleResponceDto> FutureConvert(string json)
        {
            var candles = new List<GateCandleResponceDto>();

            try
            {
                // Десериализуем как массив массивов
                var candleArrays = JsonSerializer.Deserialize<List<dynamic>>(json);

                if (candleArrays == null)
                    return candles;

                foreach (var candleObj in candleArrays)
                {
                    try
                    {
                        var candle = new GateCandleResponceDto
                        {
                            Open = candleObj.GetProperty("o").GetString() ?? "",
                            Volume = candleObj.GetProperty("v").GetInt64().ToString(),
                            OpenTime = candleObj.GetProperty("t").GetInt32(),
                            Close = candleObj.GetProperty("c").GetString() ?? "",
                            Low = candleObj.GetProperty("l").GetString() ?? "",
                            High = candleObj.GetProperty("h").GetString() ?? "",
                        };

                        candles.Add(candle);
                    }
                    catch (Exception ex)
                    {
                        // Логирование ошибки для конкретной свечи
                        Console.WriteLine($"Error parsing candle: {ex.Message}");
                    }
                }
            }
            catch (Exception ex)
            {
                // Логирование общей ошибки парсинга
                Console.WriteLine($"Error parsing JSON: {ex.Message}");
            }

            return candles;
        }

        private static decimal ParseDecimal(JsonElement element)
        {
            // Обрабатываем и числа, и строки
            return element.ValueKind switch
            {
                JsonValueKind.Number => element.GetDecimal(),
                JsonValueKind.String => decimal.Parse(
                    element.GetString()!, 
                    NumberStyles.Float, 
                    CultureInfo.InvariantCulture),
                _ => throw new InvalidOperationException(
                    $"Unsupported JSON type for decimal: {element.ValueKind}")
            };
        }

    }
}