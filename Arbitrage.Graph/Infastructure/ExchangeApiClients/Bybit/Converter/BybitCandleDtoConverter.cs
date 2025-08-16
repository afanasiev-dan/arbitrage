using System.Text.Json;
using Arbitrage.Graph.Infastructure.ExchangeApiClients.Bybit.Dto;

namespace Arbitrage.Graph.Infastructure.ExchangeApiClients.Bybit.Converter
{
    public class BybitCandleDtoConverter
    {
        public static List<BybitCandleResponceDto> Convert(string json)
        {
            var candles = new List<BybitCandleResponceDto>();

            try
            {
                // Десериализуем как массив массивов
                var candleArrays = JsonSerializer.Deserialize<BybitSpotCandleResponseDto>(json);

                if (candleArrays == null)
                    return candles;

                foreach (var candleArray in candleArrays.Result.Candles)
                {
                    try
                    {
                        if (candleArray.Count() < 7)
                            continue;

                        var candle = new BybitCandleResponceDto
                        {
                            OpenTime = long.Parse(candleArray[0]),
                            Open = candleArray[1] ?? "",
                            High = candleArray[2] ?? "",
                            Low = candleArray[3] ?? "",
                            Close = candleArray[4] ?? "",
                            Volume = candleArray[5] ?? "",
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
    }
}