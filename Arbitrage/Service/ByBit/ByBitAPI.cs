using Arbitrage;
using Arbitrage.Core.Base;
using Arbitrage.Core.Base.Enums;
using Arbitrage.Other;
using Arbitrage.Service.Base;
using Arbitrage.Service.ByBit;
using Newtonsoft.Json.Linq;
using RestSharp;
using System.Globalization;
using System.Text.Json;


public class ByBitAPI : Exchange
{
    public ByBitAPI(ExchangeAssetInfo settings) : base(settings)
    {
    }

    public async override Task Init(int size)
    {
        await base.Init(size);
    }

    public override SocketBook CreateSocketBook()
        => new ByBitSocket();

    protected async override Task<List<CoinInfo>> GetFutureCoins()
        => await GetCoins("https://api.bybit.com/v5/market/instruments-info?category=linear&status=Trading&limit=1000");

    protected async override Task<List<CoinInfo>> GetSpotCoins()
        => await GetCoins("https://api.bybit.com/v5/market/instruments-info?category=spot&status=Trading&limit=1000");

    async Task<List<CoinInfo>> GetCoins(string url)
    {
        var response = await Network.GetAsync(url, timeOut: 10000);
        var result = new List<CoinInfo>();

        using var doc = JsonDocument.Parse(response);
        var root = doc.RootElement;

        foreach (var item in root.GetProperty("result").GetProperty("list").EnumerateArray())
        {
            if (url.Contains("linear"))
            {
                var contractType = item.GetProperty("contractType").GetString();
                if (contractType != "LinearPerpetual")
                    continue;
            }
            var coin = new CoinInfo
            {
                Ticker = item.GetProperty("symbol").GetString(),
                BaseCoin = item.GetProperty("baseCoin").GetString(),
                QuoteCoin = item.GetProperty("quoteCoin").GetString(),
                //SourceExchanges = new() { Settings.Name.ToString().ToLower() }
            };
            var status = item.GetProperty("status").GetString();
            if (status != "Trading")
                Console.WriteLine(status);
            result.Add(coin);
        }
        return result;
    }

    //public override async Task UpdateAdress(List<CoinInfo> coins)
    //{
        //long serverTime = await GetServerTime();
        //long localTime = GetAccurateTimestamp();
        //long timeOffset = serverTime - localTime;
        //long timestamp = GetAccurateTimestamp() + timeOffset;
        //string recvWindow = "10000";
        //string signature = CreateSignature(secret, timestamp, key, recvWindow, "");

        //var client = new RestClient("https://api.bybit.com/v5/asset/coin/query-info");
        //var request = new RestRequest();
        //request.AddHeader("X-BAPI-API-KEY", key);
        //request.AddHeader("X-BAPI-TIMESTAMP", timestamp);
        //request.AddHeader("X-BAPI-RECV-WINDOW", recvWindow);
        //request.AddHeader("X-BAPI-SIGN", signature);
        //RestResponse response = client.Execute(request);
        //Console.WriteLine(response.Content);
    //}

    public override async Task<Dictionary<DateTime, decimal>> LoadSpotCandles(string ticker, AssetTypeEnum assetType)
        => await LoadCandles(ticker, assetType);
    public override async Task<Dictionary<DateTime, decimal>> LoadFutureCandles(string ticker, AssetTypeEnum assetType)
        => await LoadCandles(ticker, assetType);
    async Task<Dictionary<DateTime, decimal>> LoadCandles(string ticker, AssetTypeEnum assetType)
    {
        string category = assetType == AssetTypeEnum.Spot ? "spot" : "linear";
        string url = $"https://api.bybit.com/v5/market/kline?category={category}&symbol={ticker}&interval={LaunchConfig.IntervalCandle}&limit={LaunchConfig.SMALen}";

        Dictionary<DateTime, decimal> candles = new();
        string response = await client.Get(url);

        var j = JObject.Parse(response);
        var list = j["result"]?["list"]?.ToObject<List<List<string>>>();

        if (list == null)
            return candles;

        foreach (var item in list)
        {
            long timestampMs = long.Parse(item[0]);
            DateTime time = (DateTimeOffset.FromUnixTimeMilliseconds(timestampMs).UtcDateTime).AddHours(3);
            decimal close = decimal.Parse(item[4], CultureInfo.InvariantCulture);
            candles[time] = close;
        }

        return candles;
    }

    public override async Task UpdateFunding()
    {
        string url = "https://api.bybit.com/v5/market/tickers?category=linear";
        var response = await Network.GetAsync(url);
        var root = JsonDocument.Parse(response).RootElement;
        if (root.TryGetProperty("retCode", out JsonElement retCode) && retCode.GetInt32() != 0)
            return;

        var result = root.GetProperty("result");
        var list = result.GetProperty("list");
        foreach (JsonElement contract in list.EnumerateArray())
        {
            try
            {
                string ticker = contract.GetProperty("symbol").GetString();
                var crypto = GetCrypto(ticker);
                if (crypto != null)
                {
                    var fund = crypto.Funding;
                    //fund.Interval = contract.GetProperty("funding_interval").GetInt32() / 60 / 60;
                    fund.Value = F.ToDec(contract.GetProperty("fundingRate").GetString()) * 100;
                    var timeFundLong = long.Parse(contract.GetProperty("nextFundingTime").GetString());
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
