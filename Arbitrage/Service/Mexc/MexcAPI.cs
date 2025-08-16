using Arbitrage.Core.Base;
using Arbitrage.Core.Base.Enums;
using Arbitrage.Other;
using Arbitrage.Service.Base;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Arbitrage.Service.Mexc
{
    public class MexcAPI : Exchange
    {
        string secret = "4075476c4db14816b07c42b603c3f3a5";
        string key = "mx0vgl9gi78FAwavmF";

        public MexcAPI(ExchangeAssetInfo settings) : base(settings)
        {
        }

        public async override Task Init(int size)
        {
            await base.Init(size);
        }

        public override SocketBook CreateSocketBook()
            => new MexcSocket();

        protected async override Task<List<CoinInfo>> GetFutureCoins()
        {
            string url = "https://contract.mexc.com/api/v1/contract/detail";
            var response = await Network.GetAsync(url, timeOut: 10000);
            var result = new List<CoinInfo>();

            using var doc = JsonDocument.Parse(response);
            var root = doc.RootElement;
            foreach (var item in root.GetProperty("data").EnumerateArray())
            {
                var status = item.GetProperty("state").GetInt32();
                if (status != 0) continue;

                var coin = new CoinInfo
                {
                    Ticker = item.GetProperty("symbol").GetString(),
                    BaseCoin = item.GetProperty("baseCoinName").GetString(),
                    QuoteCoin = item.GetProperty("quoteCoin").GetString(),
                };
                result.Add(coin);
            }
            return result;
        }
        protected async override Task<List<CoinInfo>> GetSpotCoins()
        {
            List<(string Symbol, decimal QuoteVolume)> coins_volume = await GetSpotCoinsVolume();
            coins_volume = coins_volume.Where(x => x.QuoteVolume > 70000 ).ToList();
            var highVolumeSymbols = new HashSet<string>(coins_volume.Select(x => x.Symbol));

            string url = "https://api.mexc.com/api/v3/exchangeInfo";
            var response = await Network.GetAsync(url, timeOut: 10000);
            var result = new List<CoinInfo>();

            using var doc = JsonDocument.Parse(response);
            var root = doc.RootElement;
            foreach (var item in root.GetProperty("symbols").EnumerateArray())
            {
                var status = item.GetProperty("status").GetString();
                if (status != "1") continue;

                var symbol = item.GetProperty("symbol").GetString();
                if (!highVolumeSymbols.Contains(symbol)) continue; // отфильтровываем по объему

                var coin = new CoinInfo
                {
                    Ticker = symbol,
                    BaseCoin = item.GetProperty("baseAsset").GetString(),
                    QuoteCoin = item.GetProperty("quoteAsset").GetString(),
                    //SourceExchanges = new() { Settings.Name.ToString().ToLower() }
                };
                result.Add(coin);
            }
            return result;
        }

        async Task<List<(string, decimal)>> GetSpotCoinsVolume()
        {
            //string url_volume = "https://api.mexc.com/api/v3/ticker/24hr";
            //var response = await Network.GetAsync(url_volume);
            string response = File.ReadAllText("Data/MEXC_SPOT_volume.txt");
            var result = new List<(string Symbol, decimal QuoteVolume)>();

            using var doc = JsonDocument.Parse(response);
            var root = doc.RootElement;

            foreach (var item in root.EnumerateArray())
            {
                var symbol = item.GetProperty("symbol").GetString();
                var quoteVolume = F.ToDec(item.GetProperty("quoteVolume").GetString());

                var lastPrice = F.ToDec(item.GetProperty("lastPrice").GetString());
                result.Add((symbol, quoteVolume));
            }

            // сортировка по убыванию quoteVolume
            var sorted = result.OrderByDescending(x => x.QuoteVolume).ToList();
            return result;
        }

        public override async Task<Dictionary<DateTime, decimal>> LoadSpotCandles(string ticker, AssetTypeEnum assetType)
        {
            var url = $"https://api.mexc.com/api/v3/klines?symbol={ticker}&interval={LaunchConfig.IntervalCandle}m&limit={LaunchConfig.SMALen}";
            var response = await Network.GetAsync(url);
            var json = JArray.Parse(response);

            var candles = new Dictionary<DateTime, decimal>();

            foreach (var candle in json)
            {
                long timestamp = (long)candle[0]; // Время открытия (мс)
                decimal close = decimal.Parse(candle[4].ToString(), CultureInfo.InvariantCulture);

                var timeUtc = DateTimeOffset.FromUnixTimeMilliseconds(timestamp).UtcDateTime;
                var moscowTime = TimeZoneInfo.ConvertTimeFromUtc(timeUtc, TimeZoneInfo.FindSystemTimeZoneById("Russian Standard Time"));

                candles[moscowTime] = close;
            }

            return candles;
        }
        public override async Task<Dictionary<DateTime, decimal>> LoadFutureCandles(string ticker, AssetTypeEnum assetType)
        {
            var candles = new Dictionary<DateTime, decimal>();

            string url = $"https://contract.mexc.com/api/v1/contract/kline/{ticker}?interval=Min{LaunchConfig.IntervalCandle}&limit={LaunchConfig.SMALen}";

            var response = await Network.GetAsync(url);
            var json = JObject.Parse(response);

            if (json["success"]?.Value<bool>() != true)
            {
                Console.WriteLine($"[{Settings.Print()}] много запросов, ждем 5 сек");
                await Task.Delay(5000);
                return await LoadFutureCandles(ticker, assetType);
            }

            var data = json["data"];

            var times = data["time"]?.ToObject<List<long>>();
            var closes = data["close"]?.ToObject<List<decimal>>();

            if (times == null || closes == null || times.Count != closes.Count)
                throw new Exception("Invalid response structure from MEXC");

            for (int i = 0; i < times.Count; i++)
            {
                // timestamp в секундах, переводим в DateTime и в московское время
                var timeUtc = DateTimeOffset.FromUnixTimeSeconds(times[i]).UtcDateTime;
                var timeMoscow = TimeZoneInfo.ConvertTimeFromUtc(timeUtc, TimeZoneInfo.FindSystemTimeZoneById("Russian Standard Time"));

                candles[timeMoscow] = closes[i];
            }

            return candles;
        }

        public override async Task UpdateFunding()
        {
            string url = "https://contract.mexc.com/api/v1/contract/funding_rate/";
            var response = await Network.GetAsync(url);
            var root = JsonDocument.Parse(response).RootElement;

            if (!root.TryGetProperty("success", out JsonElement success) || !success.GetBoolean())
                return;
            if (!root.TryGetProperty("code", out JsonElement retCode) || retCode.GetInt32() != 0)
                return;

            var list = root.GetProperty("data");
            foreach (JsonElement contract in list.EnumerateArray())
            {
                try
                {
                    string ticker = contract.GetProperty("symbol").GetString();
                    var crypto = GetCrypto(ticker);
                    if (crypto != null)
                    {
                        var fund = crypto.Funding;
                        fund.Interval = contract.GetProperty("collectCycle").GetInt32();
                        fund.Value = contract.GetProperty("fundingRate").GetDecimal() * 100;

                        var timeFundLong = contract.GetProperty("nextSettleTime").GetInt64();
                        fund.TimePay = DateTimeOffset.FromUnixTimeMilliseconds(timeFundLong).ToLocalTime().DateTime;
                        fund.Type = FundingType.Float;
                    };
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Ошибка при парсинге контракта: {ex.Message}");
                }
            }
        }
    }
}
