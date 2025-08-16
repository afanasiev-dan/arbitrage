
using System.Text.Json;
using Arbitrage.Graph.Infastructure.ExchangeApiClients.LBank.Dto;

namespace Arbitrage.Graph.Infastructure.ExchangeApiClients.LBank.Converter
{
    public class LBankCandleDtoConverter
    {
        public static List<LBankCandleResponceDto> Convert(string json)
        {
            var candles = new List<LBankCandleResponceDto>();

            try
            {
                var responceData = JsonSerializer.Deserialize<LBankSpotDataResponseDto>(json);

                if (responceData == null)
                    return candles;

                foreach (var candleArray in responceData.Data)
                {
                    try
                    {
                        var candle = new LBankCandleResponceDto
                        {
                            OpenTime = int.Parse(candleArray[0].ToString()),
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