using System.Globalization;
using System.Text.Json;
using Arbitrage.Graph.Infastructure.ExchangeApiClients.Mexc.Dto;

namespace Arbitrage.Graph.Infastructure.ExchangeApiClients.Mexc.Converter
{
    public class MexcCandleDtoConverter
    {
        public static List<MexcCandleResponceDto> SpotConvert(string json)
        {
            using JsonDocument doc = JsonDocument.Parse(json);
            JsonElement root = doc.RootElement;
            
            var candles = new List<MexcCandleResponceDto>();
            
            foreach (JsonElement candleElement in root.EnumerateArray())
            {
                try
                {
                    // Проверяем, что в массиве достаточно элементов
                    if (candleElement.GetArrayLength() < 8)
                        continue;
                    
                    var candle = new MexcCandleResponceDto
                    {
                        OpenTime = candleElement[0].GetInt64(),
                        Open = candleElement[1].GetString() ?? "",
                        High = candleElement[2].GetString() ?? "",
                        Low = candleElement[3].GetString() ?? "",
                        Close = candleElement[4].GetString() ?? "",
                        Volume = candleElement[5].GetString() ?? "",
                        CloseTime = candleElement[6].GetInt64(),
                        
                        // Безопасное преобразование строки в decimal
                        QuoteAssetVolume = ParseDecimal(candleElement[7])
                    };
                    
                    candles.Add(candle);
                }
                catch (Exception ex)
                {
                    // Логирование ошибки (добавьте реальное логирование)
                    Console.WriteLine($"Error parsing candle: {ex.Message}");
                }
            }
            
            return candles;
        }

        public static List<MexcCandleResponceDto> FuturesConvert(string json)
        {
           var response = JsonSerializer.Deserialize<MexcFuturesResponse>(json);
            
            if (!response.Success || response.Data == null)
            {
                throw new InvalidOperationException("Invalid futures response");
            }

            var data = response.Data;
            var candles = new List<MexcCandleResponceDto>();
            
            // Проверяем согласованность массивов
            int length = data.Time.Length;
            if (length == 0 || 
                data.Open.Length != length ||
                data.High.Length != length ||
                data.Low.Length != length ||
                data.Close.Length != length ||
                data.Vol.Length != length ||
                data.Amount.Length != length)
            {
                throw new InvalidDataException("Inconsistent array lengths in futures response");
            }

            for (int i = 0; i < length; i++)
            {
                try
                {
                    candles.Add(new MexcCandleResponceDto
                    {
                        OpenTime = data.Time[i] * 1000, // Конвертация в миллисекунды
                        Open = data.Open[i].ToString(CultureInfo.InvariantCulture),
                        High = data.High[i].ToString(CultureInfo.InvariantCulture),
                        Low = data.Low[i].ToString(CultureInfo.InvariantCulture),
                        Close = data.Close[i].ToString(CultureInfo.InvariantCulture),
                        Volume = data.Vol[i].ToString(CultureInfo.InvariantCulture),
                        CloseTime = 0, // Не предоставляется API
                        QuoteAssetVolume = data.Amount[i]
                    });
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error parsing futures candle at index {i}: {ex.Message}");
                }
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