using Arbitrage.Core;
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

namespace Arbitrage.Service.HTX
{
    internal class HTXAPI : Exchange
    {
        public HTXAPI(ExchangeAssetInfo settings) : base(settings)
        {
        }

        public async override Task Init(int size)
        {
            await base.Init(size);
        }

        public override SocketBook CreateSocketBook()
            => new HTXSocket();

        public override async Task<Dictionary<DateTime, decimal>> LoadFutureCandles(string ticker, AssetTypeEnum assetType)
        {
            string url = $"https://api.hbdm.com/linear-swap-ex/market/history/kline?contract_code={ticker}&period=15min&size={LaunchConfig.SMALen}";
            Dictionary<DateTime, decimal> candles = new();

            string response = await Network.GetAsync(url);
            var j = JObject.Parse(response);

            var data = j["data"]?.ToObject<List<JObject>>();
            if (data == null || data.Count == 0)
                return candles;

            foreach (var item in data)
            {
                try
                {
                    long timestamp = item.Value<long>("id"); // это в секундах
                    DateTime time = DateTimeOffset.FromUnixTimeSeconds(timestamp)
                        .ToOffset(TimeSpan.FromHours(3)) // МСК
                        .DateTime;

                    decimal close = item.Value<decimal>("close");

                    // гарантируем уникальные ключи
                    if (!candles.ContainsKey(time))
                        candles[time] = close;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Ошибка при обработке свечи: {ex.Message}");
                }
            }

            // упорядочим по убыванию (по необходимости)
            return candles.OrderByDescending(x => x.Key)
                          .Take(LaunchConfig.SMALen)
                          .OrderBy(x => x.Key) // вернуть в хрон. порядок
                          .ToDictionary(x => x.Key, x => x.Value);
        }
        public override async Task<Dictionary<DateTime, decimal>> LoadSpotCandles(string ticker, AssetTypeEnum assetType)
        {
            string category = assetType == AssetTypeEnum.Spot ? "spot" : "linear";
            string url = $"https://api.huobi.pro/market/history/kline?period={LaunchConfig.IntervalCandle}min&size={LaunchConfig.SMALen}&symbol={ticker}";

            Dictionary<DateTime, decimal> candles = new();
            string response = await Network.GetAsync(url);
            var j = JObject.Parse(response);
            var data = j["data"]?.ToObject<List<JObject>>();
            if (data == null)
                return candles;

            foreach (var item in data)
            {
                try
                {
                    long timestampMs = item.Value<long>("id");
                    DateTime time = DateTimeOffset.FromUnixTimeSeconds(timestampMs)
                        .ToOffset(TimeSpan.FromHours(3))
                        .DateTime;
                    decimal close = item.Value<decimal>("close");
                    candles[time] = close;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Ошибка при обработке свечи: {ex.Message}");
                }
            }

            return candles;
        }

        protected async override Task<List<CoinInfo>> GetFutureCoins()
        {
            string url = "https://api.hbdm.com/linear-swap-api/v1/swap_contract_info";
            var response = await Network.GetAsync(url, timeOut: 10000);
            var result = new List<CoinInfo>();

            using var doc = JsonDocument.Parse(response);
            var root = doc.RootElement;

            foreach (var item in root.GetProperty("data").EnumerateArray())
            {
                string pair = item.GetProperty("pair").GetString(); // Пример: "BTC-USDT"
                if (string.IsNullOrEmpty(pair) || item.GetProperty("contract_status").GetInt32() != 1)
                    continue;

                var parts = pair.Split('-');
                if (parts.Length != 2) continue;

                var coin = new CoinInfo
                {
                    Ticker = item.GetProperty("contract_code").GetString(), // Пример: BTC-USDT
                    BaseCoin = G.Map(parts[0]),
                    QuoteCoin = G.Map(parts[1]),
                    Multiplier = item.GetProperty("contract_size").GetDecimal(),
                    //SourceExchanges = new() { Settings.Name.ToString().ToLower() }
                };

                result.Add(coin);
            }
            return result;
        }
        protected async override Task<List<CoinInfo>> GetSpotCoins()
        {
            string url = "https://api.huobi.pro/v1/common/symbols";
            var response = await Network.GetAsync(url, timeOut: 10000);
            var result = new List<CoinInfo>();

            using var doc = JsonDocument.Parse(response);
            var root = doc.RootElement;

            foreach (var item in root.GetProperty("data").EnumerateArray())
            {
                var state = item.GetProperty("state").GetString();
                if (state != "online")
                    continue;

                var baseCoin = item.GetProperty("base-currency").GetString().ToUpper();
                var quoteCoin = item.GetProperty("quote-currency").GetString().ToUpper();
                var symbol = item.GetProperty("symbol").GetString();

                var coin = new CoinInfo
                {
                    Ticker = symbol,
                    BaseCoin = G.Map(baseCoin),
                    QuoteCoin = G.Map(quoteCoin),
                    //SourceExchanges = new() { Settings.Name.ToString().ToLower() }
                };

                result.Add(coin);
            }

            return result;
        }


        public override async Task UpdateFunding()
        {
            string url = "https://api.hbdm.com/linear-swap-api/v1/swap_batch_funding_rate";
            var response = await Network.GetAsync(url);
            var root = JsonDocument.Parse(response).RootElement;

            if (!root.TryGetProperty("status", out JsonElement success) || success.GetString() != "ok")
                return;

            var list = root.GetProperty("data");
            foreach (JsonElement contract in list.EnumerateArray())
            {
                try
                {
                    string ticker = contract.GetProperty("contract_code").GetString();
                    var crypto = GetCrypto(ticker);
                    if (crypto != null)
                    {
                        var fund = crypto.Funding;
                        //fund.Interval = contract.GetProperty("collectCycle").GetInt32();
                        fund.Value = F.ToDec(contract.GetProperty("funding_rate").GetString()) * 100;

                        var timeFundLong = long.Parse(contract.GetProperty("funding_time").GetString());
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
