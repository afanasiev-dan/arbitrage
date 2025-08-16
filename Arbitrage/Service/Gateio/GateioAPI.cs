using Arbitrage.Core.Base;
using Arbitrage.Core.Base.Enums;
using Arbitrage.Other;
using Arbitrage.Service.Base;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Arbitrage.Service.Gateio
{
    internal class GateioAPI : Exchange
    {
        public GateioAPI(ExchangeAssetInfo settings) : base(settings)
        {
        }

        public override SocketBook CreateSocketBook()
            => new GateioSocket();

        public override async Task<Dictionary<DateTime, decimal>> LoadFutureCandles(string ticker, AssetTypeEnum assetType)
        {
            var candles = new Dictionary<DateTime, decimal>();

            string url = $"https://api.gateio.ws/api/v4/futures/usdt/candlesticks?contract={ticker}&interval={LaunchConfig.IntervalCandle}m&limit={LaunchConfig.SMALen}";
            var response = await Network.GetAsync(url);
            var array = JArray.Parse(response);

            foreach (var item in array)
            {
                long timestamp = item.Value<long>("t");
                DateTime time = DateTimeOffset.FromUnixTimeSeconds(timestamp).ToOffset(TimeSpan.FromHours(3)).DateTime; // МСК
                decimal close = item.Value<decimal>("c");

                candles[time] = close;
            }

            return candles;
        }
        public override async Task<Dictionary<DateTime, decimal>> LoadSpotCandles(string ticker, AssetTypeEnum assetType)
        {
            var candles = new Dictionary<DateTime, decimal>();

            string url = $"https://api.gateio.ws/api/v4/spot/candlesticks?currency_pair={ticker}&interval={LaunchConfig.IntervalCandle}m&limit={LaunchConfig.SMALen}";
            var response = await Network.GetAsync(url);
            var array = JArray.Parse(response);

            foreach (var item in array)
            {
                long timestamp = long.Parse(item[0].ToString());
                decimal close = decimal.Parse(item[2].ToString(), CultureInfo.InvariantCulture);

                var utcTime = DateTimeOffset.FromUnixTimeSeconds(timestamp).UtcDateTime;
                var moscowTime = TimeZoneInfo.ConvertTimeFromUtc(utcTime, TimeZoneInfo.FindSystemTimeZoneById("Russian Standard Time"));

                candles[moscowTime] = close;
            }

            return candles;
        }

        protected override async Task<List<CoinInfo>> GetFutureCoins()
        {
            string url = "https://api.gateio.ws/api/v4/futures/usdt/contracts";
            var response = await Network.GetAsync(url, timeOut: 10000);
            var result = new List<CoinInfo>();

            using var doc = JsonDocument.Parse(response);
            var root = doc.RootElement;

            foreach (var item in root.EnumerateArray())
            {
                var status = item.GetProperty("in_delisting").GetBoolean();
                if (status) continue;

                var coin = new CoinInfo
                {
                    Ticker = item.GetProperty("name").GetString(),
                    Multiplier = F.ToDec(item.GetProperty("quanto_multiplier").GetString())
                    //SourceExchanges = new() { Settings.Name.ToString().ToLower() }
                };
                coin.BaseCoin = coin.Ticker.Split('_')[0];
                coin.QuoteCoin = coin.Ticker.Split('_')[1];
                result.Add(coin);
            }

            return result;
        }
        protected override async Task<List<CoinInfo>> GetSpotCoins()
        {
            string url_volume = "https://api.gateio.ws/api/v4/spot/tickers";
            var response = await Network.GetAsync(url_volume);

            var doc = JsonDocument.Parse(response);
            var root = doc.RootElement;

            var result = new List<CoinInfo>();
            foreach (var item in root.EnumerateArray())
            {
                var symbol = item.GetProperty("currency_pair").GetString();
                var abs = symbol.Split("_");
                var coin = new CoinInfo
                {
                    Ticker = symbol,
                    BaseCoin = abs[0],
                    QuoteCoin = abs[1],
                    Volume24H = F.ToDec(item.GetProperty("quote_volume").GetString())
                    //SourceExchanges = new() { Settings.Name.ToString().ToLower() }
                };
                result.Add(coin);
            }
            result = result.OrderByDescending(x => x.Volume24H).Take(1500).ToList();
            return result;
        }
        async Task<List<(string Symbol, decimal QuoteVolume)>> GetSpotCoinsVolume()
        {
            string url_volume = "https://api.gateio.ws/api/v4/spot/tickers";
            var response = await Network.GetAsync(url_volume);
            var result = new List<(string Symbol, decimal QuoteVolume)>();

            var doc = JsonDocument.Parse(response);
            var root = doc.RootElement;

            foreach (var item in root.EnumerateArray())
            {
                var symbol = item.GetProperty("currency_pair").GetString();
                var quoteVolume = F.ToDec(item.GetProperty("quote_volume").GetString());
                result.Add((symbol, quoteVolume));
            }

            return result.OrderByDescending(x => x.QuoteVolume).ToList();
        }

        public override async Task UpdateFunding()
        {
            string url = "https://api.gateio.ws/api/v4/futures/usdt/contracts";
            var response = await Network.GetAsync(url);
            var root = JsonDocument.Parse(response).RootElement;
            foreach (JsonElement contract in root.EnumerateArray())
            {
                try
                {
                    string ticker = contract.GetProperty("name").GetString();
                    var crypto = GetCrypto(ticker);
                    if (crypto != null)
                    {
                        var fund = crypto.Funding;
                        fund.Interval = contract.GetProperty("funding_interval").GetInt32() / 60 / 60;
                        fund.Value = F.ToDec(contract.GetProperty("funding_rate_indicative").GetString()) * 100;
                        var timeFundLong = contract.GetProperty("funding_next_apply").GetInt64();
                        fund.TimePay = DateTimeOffset.FromUnixTimeSeconds(timeFundLong).ToLocalTime().DateTime;
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
