using System.Text.Json;
using Arbitrage.Graph.Infastructure.ExchangeApiClients.KuCoin.Dto;

namespace Arbitrage.Graph.Infastructure.ExchangeApiClients.KuCoin
{
    public class KuCoinCandleDtoConverter
    {
        public static List<KuCoinCandleResponceDto> SpotConvert(string json)
        {
            var candles = new List<KuCoinCandleResponceDto>();

            try
            {
                var responceData = JsonSerializer.Deserialize<KuCoinApiSpotResponseDto>(json);

                if (responceData == null)
                    return candles;

                foreach (var candleArray in responceData.Candles)
                {
                    try
                    {
                        var candle = new KuCoinCandleResponceDto
                        {
                            OpenTime = int.Parse(candleArray[0]),
                            Open = candleArray[1],
                            High = candleArray[2],
                            Low = candleArray[3],
                            Close = candleArray[4],
                            Volume = candleArray[5],
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


        public static List<KuCoinCandleResponceDto> FutureConvert(string json)
        {
            var candles = new List<KuCoinCandleResponceDto>();

            try
            {
                var responceData = JsonSerializer.Deserialize<KuCoinApiFutureResponseDto>(json);

                if (responceData == null)
                    return candles;

                foreach (var candleArray in responceData.Candles)
                {
                    try
                    {
                        var candle = new KuCoinCandleResponceDto
                        {
                            OpenTime = long.Parse(candleArray[0].ToString()),
                            Open = candleArray[1].ToString() ?? "",
                            High = candleArray[2].ToString() ?? "",
                            Low = candleArray[3].ToString() ?? "",
                            Close = candleArray[4].ToString() ?? "",
                            Volume = candleArray[5].ToString() ?? "",
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