using Arbitrage.Core.Base;
using Arbitrage.Core.Base.Enums;
using Arbitrage.Other;
using Arbitrage.Service.Base;
using System.Text.Json;

namespace Arbitrage.Service.LBank
{
    public class LBankAPI : Exchange
    {
        public LBankAPI(ExchangeAssetInfo settings) : base(settings)
        {
        }

        public async override Task Init(int size)
        {
            await base.Init(size);
        }

        public override SocketBook CreateSocketBook()
            => new LBankSocketV2();

        public override async Task<Dictionary<DateTime, decimal>> LoadSpotCandles(string ticker, AssetTypeEnum assetType)
        {
            long unixTimeSeconds = DateTimeOffset.UtcNow.AddMinutes(-100 * 15).ToUnixTimeSeconds();
            string url = $"https://api.lbkex.com/v2/kline.do?symbol={ticker}&type=minute15&size=100&time={unixTimeSeconds}";

            Dictionary<DateTime, decimal> candles = new();
            var response = await client.Get(url);

            using JsonDocument doc = JsonDocument.Parse(response);
            var root = doc.RootElement;

            if (root.TryGetProperty("data", out var data) && data.ValueKind == JsonValueKind.Array)
            {
                foreach (var item in data.EnumerateArray())
                {
                    if (item.ValueKind == JsonValueKind.Array && item.GetArrayLength() >= 6)
                    {
                        long timestamp = item[0].GetInt64();
                        decimal close = item[4].GetDecimal();
                        DateTime time = DateTimeOffset.FromUnixTimeSeconds(timestamp).UtcDateTime.AddHours(3);
                        candles[time] = close;
                    }
                }
            }

            return candles;
        }

        public override async Task<Dictionary<DateTime, decimal>> LoadFutureCandles(string ticker, AssetTypeEnum assetType)
        {
            Dictionary<DateTime, decimal> candles = new();
            return candles;
        }

        protected async override Task<List<CoinInfo>> GetFutureCoins()
        {
            string url = "https://lbkperp.lbank.com/cfd/openApi/v1/pub/instrument?productGroup=SwapU";
            var response = await Network.GetAsync(url);

            var result = new List<CoinInfo>();
            var root = JsonDocument.Parse(response).RootElement;
            foreach (var element in root.GetProperty("data").EnumerateArray())
            {
                var ticker = element.GetProperty("symbol").GetString();

                var coin = new CoinInfo
                {
                    Ticker = element.GetProperty("symbol").GetString(),
                    BaseCoin = element.GetProperty("baseCurrency").GetString(),
                    QuoteCoin = element.GetProperty("clearCurrency").GetString(),
                    PriceTick = element.GetProperty("priceTick").GetDecimal(),
                    //SourceExchanges = new() { Settings.Name.ToString().ToLower() }
                };
                result.Add(coin);
            }
            return result;
        }

        protected async override Task<List<CoinInfo>> GetSpotCoins()
        {
            string url = "https://api.lbkex.com/v2/currencyPairs.do";
            var response = await Network.GetAsync(url);
            var result = new List<CoinInfo>();

            using var doc = JsonDocument.Parse(response);
            var root = doc.RootElement;
            var data = root.GetProperty("data");

            foreach (var item in data.EnumerateArray())
            {
                var ticker = item.GetString();
                if (string.IsNullOrEmpty(ticker) || !ticker.Contains("_"))
                    continue;

                var parts = ticker.Split('_');
                var baseCoin = parts[0].ToUpper();
                var quoteCoin = parts[1].ToUpper();

                var coin = new CoinInfo
                {
                    Ticker = ticker.ToLower(),
                    BaseCoin = baseCoin,
                    QuoteCoin = quoteCoin,
                    //SourceExchanges = new() { Settings.Name.ToString().ToLower() }
                };
                result.Add(coin);
            }

            return result;
        }

        public override async Task UpdateFunding()
        {
            string url = "https://lbkperp.lbank.com/cfd/openApi/v1/pub/marketData?productGroup=SwapU";
            var response = await Network.GetAsync(url);
            var root = JsonDocument.Parse(response).RootElement;
            if (root.TryGetProperty("error_code", out JsonElement retCode) && retCode.GetInt32() != 0)
                return;

            var list = root.GetProperty("data");
            foreach (JsonElement contract in list.EnumerateArray())
            {
                try
                {
                    string ticker = contract.GetProperty("symbol").GetString();
                    var crypto = GetCrypto(ticker);
                    var volume = F.ToDec(contract.GetProperty("volume").GetString());
                    if (crypto != null && volume != 0)
                    {
                        var fund = crypto.Funding;

                        fund.Interval = contract.GetProperty("positionFeeTime").GetInt32() / 60 / 60;
                        fund.Value = F.ToDec(contract.GetProperty("fundingRate").GetString()) * 100;

                        var timeFundLong = contract.GetProperty("nextFeeTime").GetInt64();
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
