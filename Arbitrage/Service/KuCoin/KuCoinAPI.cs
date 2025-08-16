using System;
using System.Globalization;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Timers;
using Arbitrage;
using Arbitrage.Core;
using Arbitrage.Core.Base;
using Arbitrage.Core.Base.Enums;
using Arbitrage.Other;
using Arbitrage.Service.Base;
using Newtonsoft.Json.Linq;
using Timer = System.Timers.Timer;

class KuCoinAPI : Exchange
{
    public KuCoinAPI(ExchangeAssetInfo settings) : base(settings)
    {
    }

    public override async Task Init(int size)
    {
        await base.Init(size);
    }

    public override SocketBook CreateSocketBook()
        => new KuCoinSocket();

    //docs: https://www.kucoin.com/docs-new/rest/futures-trading/market-data/get-all-symbols
    protected override async Task<List<CoinInfo>> GetFutureCoins()
     => await GetCoins("https://api-futures.kucoin.com/api/v1/contracts/active");

    protected override async Task<List<CoinInfo>> GetSpotCoins()
    => await GetCoins("https://api.kucoin.com/api/v2/symbols");

    async Task<List<CoinInfo>> GetCoins(string url)
    {
        var response = await Network.GetAsync(url, timeOut: 10000);
        var result = new List<CoinInfo>();

        using var doc = JsonDocument.Parse(response);
        var root = doc.RootElement;

        foreach (var item in root.GetProperty("data").EnumerateArray())
        {
            var coin = new CoinInfo
            {
                Ticker = item.GetProperty("symbol").GetString(),
                BaseCoin = G.Map(item.GetProperty("baseCurrency").GetString()),
                QuoteCoin = G.Map(item.GetProperty("quoteCurrency").GetString()),
                Multiplier = Settings.AssetType == AssetTypeEnum.Futures ? item.GetProperty("multiplier").GetDecimal() : 1
            };
            //if (url.Contains("futures"))
            //{
            //    coin.SourceExchanges = item.GetProperty("sourceExchanges")
            //          .EnumerateArray()
            //          .Select(e => e.GetString().ToLower())
            //          .ToList();
            //}
            //else
            //{
            //    coin.SourceExchanges.Add($"{Settings.Name}".ToLower());
            //}
            result.Add(coin);
        }
        return result;
    }

    //public override async Task UpdateAdress(List<CoinInfo> coins)
    //{
    //    string url = "https://api.kucoin.com/api/v3/currencies";
    //    var response = await Network.GetAsync(url);
    //    using var doc = JsonDocument.Parse(response);
    //    var root = doc.RootElement;

    //    foreach (var item in root.GetProperty("data").EnumerateArray())
    //    {
    //        var Ticker = item.GetProperty("currency").GetString();
    //        var coin = coins.Find(x => x.BaseCoin == Ticker);
    //        if (coin == null) continue;
    //        //if (item.TryGetProperty("chains", out var chains) && chains.GetArrayLength() > 0)
    //        //{
    //        //    foreach (var chain in chains.EnumerateArray())
    //        //    {
    //        //        if (chain.TryGetProperty("contractAddress", out var contractProp))
    //        //        {
    //        //            var contract = contractProp.GetString();
    //        //            if (!string.IsNullOrEmpty(contract))
    //        //                coin.Adresses.Add(contract);
    //        //        }
    //        //    }
    //        //}

    //    }
    //}

    public override async Task<Dictionary<DateTime, decimal>> LoadSpotCandles(string ticker, AssetTypeEnum assetType)
    => await LoadCandles(ticker, assetType);
    public override async Task<Dictionary<DateTime, decimal>> LoadFutureCandles(string ticker, AssetTypeEnum assetType)
        => await LoadCandles(ticker, assetType);
    async Task<Dictionary<DateTime, decimal>> LoadCandles(string ticker, AssetTypeEnum assetType)
    {
        string url = assetType == AssetTypeEnum.Spot ?
                     $"https://api.kucoin.com/api/v1/market/candles?type={LaunchConfig.IntervalCandle}min&symbol={ticker}&limit={LaunchConfig.SMALen}" :
                     $"https://api-futures.kucoin.com/api/v1/kline/query?symbol={ticker}&granularity={LaunchConfig.IntervalCandle}&limit={LaunchConfig.SMALen}";

        Dictionary<DateTime, decimal> candles = new();
        var response = await client.Get(url);
        var jObj = JObject.Parse(response);
        var list = jObj["data"];
        if (list != null)
        {
            foreach (var item in list)
            {
                var arr = item.ToObject<List<string>>();
                try
                {
                    var timestamp = long.Parse(arr[0]);
                    var close = F.ToDec(arr[assetType == AssetTypeEnum.Spot ? 2 : 4]);
                    DateTime time = assetType == AssetTypeEnum.Spot ?
                        DateTimeOffset.FromUnixTimeSeconds(timestamp).UtcDateTime.AddHours(3) :
                        DateTimeOffset.FromUnixTimeMilliseconds(timestamp).UtcDateTime.AddHours(3);
                    candles[time] = close;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("dsa");
                }
            }
        }
        return candles;
    }

    public override async Task UpdateFunding()
    {
        string url = "https://api-futures.kucoin.com/api/v1/contracts/active";
        var response = await Network.GetAsync(url);
        var root = JsonDocument.Parse(response).RootElement;
        if (root.TryGetProperty("code", out JsonElement retCode) && retCode.GetString() != "200000")
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
                    fund.Interval = contract.GetProperty("fundingRateGranularity").GetInt32() / 1000 / 60 / 60;
                    fund.Value = contract.GetProperty("fundingFeeRate").GetDecimal() * 100;
                    var timeFundLong = long.Parse(contract.GetProperty("nextFundingRateTime").GetInt32().ToString());

                    var unixTime = DateTimeOffset.FromUnixTimeMilliseconds(timeFundLong).LocalDateTime;
                    fund.TimePay = DateTime.Now.AddMilliseconds(timeFundLong);

                    fund.Type = FundingType.Fix;
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при парсинге контракта: {ex.Message}");
            }
        }
    }


    //private static HttpClient client = new HttpClient();
    //private static Timer timer;

    //// Настройки API
    //private static string _apiKey = "67e9a9fb19d2fd0001be11f8";
    //private static string _apiSecret = "b4e09bfa-ba32-4697-94fa-508dd7840ac1";
    //private static string _apiPassphrase = "15351535";

    //// API Endpoints
    //private const string SpotApiUrl = "https://api.kucoin.com/api/v1/market/orderbook/level1?symbol=REZ-USDT";
    //private const string SpotOrderBookUrl = "https://api.kucoin.com/api/v1/market/orderbook/level2_20?symbol=REZ-USDT";
    //private const string FuturesApiUrl = "https://api-futures.kucoin.com/api/v1/contracts/REZUSDTM";
    //private const string FuturesOrderBookUrl = "https://api-futures.kucoin.com/api/v1/level2/snapshot?symbol=REZUSDTM";
    //private const string FundingHistoryUrl = "https://api-futures.kucoin.com/api/v1/contract/funding-rates";
    //private const string FundingUrl = "https://api-futures.kucoin.com/api/v1/funding-rate";

    //public static async Task Start()
    //{
    //    Console.WriteLine("Мониторинг разницы цен REZUSDT (спот vs фьючерс) на KuCoin");
    //    Console.WriteLine("-------------------------------------------------------");

    //    timer = new Timer(1000);
    //    //timer.Elapsed += async (sender, e) => await FetchAndDisplayPrices();
    //    //timer.Elapsed += async (sender, e) => await FetchOrderBook(SpotOrderBookUrl);
    //    //timer.Elapsed += async (sender, e) => await FetchOrderBook(FuturesOrderBookUrl);

    //    //long from = DateTimeOffset.UtcNow.AddDays(-7).ToUnixTimeMilliseconds();
    //    //long to = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
    //    //timer.Elapsed += async (sender, e) => await FetchFundingRateHistory("REZUSDTM", from, to);

    //    timer.Elapsed += async (sender, e) => await FetchCurrentFundingRate("REZUSDTM");
    //    timer.AutoReset = true;
    //    timer.Enabled = true;

    //    //await FetchAndDisplayPrices();

    //    Console.ReadKey();
    //    timer.Stop();
    //    timer.Dispose();
    //}

    //private static async Task FetchCurrentFundingRate(string symbol)
    //{
    //    try
    //    {
    //        string url = $"{FundingUrl}/{symbol}/current";
    //        var response = await client.GetAsync(url);

    //        if (!response.IsSuccessStatusCode)
    //        {
    //            Console.WriteLine($"Ошибка API: {response.StatusCode}");
    //            return;
    //        }

    //        var content = await response.Content.ReadAsStringAsync();
    //        var json = JObject.Parse(content);
    //        var data = json["data"];

    //        if (data == null)
    //        {
    //            Console.WriteLine("Нет данных о ставке финансирования.");
    //            return;
    //        }

    //        string fundingRate = data["value"].ToString();
    //        string predictedRate = data["predictedValue"].ToString();
    //        long fundingTimeMs = data["timePoint"].Value<long>(); // Время последней выплаты в миллисекундах
    //        long granularityMs = data["granularity"].Value<long>(); // Интервал в миллисекундах (8 часов)

    //        TimeSpan timeLeft = TimeSpan.FromMilliseconds(fundingTimeMs - DateTimeOffset.UtcNow.ToUnixTimeMilliseconds());
    //        TimeSpan granularityTimeSpan = TimeSpan.FromMilliseconds(granularityMs);

    //        TimeSpan updatedTimeLeft = timeLeft + granularityTimeSpan;

    //        Console.WriteLine($"Оставшееся время до следующей выплаты: {updatedTimeLeft}");
    //        Console.WriteLine($"Текущая ставка: {fundingRate}");
    //        Console.WriteLine($"Прогнозируемая ставка: {predictedRate}");
    //    }
    //    catch (Exception ex)
    //    {
    //        Console.WriteLine($"Ошибка: {ex.Message}");
    //    }
    //}
    //private static async Task FetchOrderBook(string url)
    //{
    //    try
    //    {
    //        var response = await client.GetAsync(url);
    //        if (!response.IsSuccessStatusCode)
    //        {
    //            Console.WriteLine($"Ошибка API: {response.StatusCode}");
    //            return;
    //        }

    //        var content = await response.Content.ReadAsStringAsync();
    //        var json = JObject.Parse(content);

    //        var bids = json["data"]?["bids"];
    //        var asks = json["data"]?["asks"];

    //        if (bids == null || asks == null)
    //        {
    //            Console.WriteLine("Некорректный JSON");
    //            return;
    //        }

    //        Console.WriteLine("\nTop 5 Bid Orders:");
    //        foreach (var bid in bids.Take(5))
    //            Console.WriteLine($"Цена: {bid[0]}, Кол-во: {bid[1]}");

    //        Console.WriteLine("\nTop 5 Ask Orders:");
    //        foreach (var ask in asks.Take(5))
    //            Console.WriteLine($"Цена: {ask[0]}, Кол-во: {ask[1]}");

    //    }
    //    catch (Exception ex)
    //    {
    //        Console.WriteLine($"Ошибка: {ex.Message}");
    //    }
    //}
    //private static async Task FetchFundingRateHistory(string symbol, long from, long to)
    //{
    //    try
    //    {
    //        string url = $"{FundingHistoryUrl}?symbol={symbol}&from={from}&to={to}";
    //        var response = await client.GetAsync(url);

    //        if (!response.IsSuccessStatusCode)
    //        {
    //            Console.WriteLine($"Ошибка API: {response.StatusCode}");
    //            return;
    //        }

    //        var content = await response.Content.ReadAsStringAsync();
    //        var json = JObject.Parse(content);
    //        var history = json["data"];

    //        if (history == null || !history.HasValues)
    //        {
    //            Console.WriteLine("Нет данных о ставках финансирования.");
    //            return;
    //        }

    //        Console.WriteLine("📌 История ставок финансирования:");
    //        foreach (var entry in history)
    //        {
    //            var fundingRate = entry["fundingRate"]?.ToString();
    //            var timeStamp = entry["timepoint"]?.ToString();

    //            if (fundingRate != null && timeStamp != null)
    //            {
    //                long unixTime = long.Parse(timeStamp);
    //                DateTime fundingDate = DateTimeOffset.FromUnixTimeMilliseconds(unixTime).UtcDateTime;
    //                Console.WriteLine($"{fundingDate:yyyy-MM-dd HH:mm:ss} UTC | Funding Rate: {fundingRate}");
    //            }
    //        }
    //    }
    //    catch (Exception ex)
    //    {
    //        Console.WriteLine($"Ошибка: {ex.Message}");
    //    }
    //}
    //private static async Task FetchAndDisplayPrices()
    //{
    //    try
    //    {
    //        // Получаем спотовую цену
    //        decimal spotPrice = await GetPrice(SpotApiUrl, "data.price");

    //        // Получаем фьючерсную цену
    //        decimal futuresPrice = await GetPrice(FuturesApiUrl, "data.markPrice");

    //        // Рассчитываем разницу
    //        decimal difference = futuresPrice - spotPrice;
    //        decimal differencePercent = (difference / spotPrice) * 100;

    //        Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] Спот: {spotPrice:F6} | Фьючерс: {futuresPrice:F6} | Разница: {difference:F6} ({differencePercent:F2}%)");
    //    }
    //    catch (Exception ex)
    //    {
    //        Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] Ошибка: {ex.Message}");
    //    }
    //}
    //private static async Task<decimal> GetPrice(string url, string jsonPath)
    //{
    //    var request = await CreateSignedRequest(HttpMethod.Get, url);
    //    var response = await client.SendAsync(request);

    //    if (!response.IsSuccessStatusCode)
    //    {
    //        var errorContent = await response.Content.ReadAsStringAsync();
    //        throw new Exception($"API Error: {response.StatusCode} - {errorContent}");
    //    }

    //    var content = await response.Content.ReadAsStringAsync();
    //    var json = JObject.Parse(content);

    //    var priceToken = json.SelectToken(jsonPath);
    //    if (priceToken == null)
    //        throw new Exception("Invalid JSON structure");

    //    return priceToken.Value<decimal>();
    //}

    //private static async Task<HttpRequestMessage> CreateSignedRequest(HttpMethod method, string url)
    //{
    //    var request = new HttpRequestMessage(method, url);
    //    var timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString();
    //    var uri = new Uri(url);

    //    // Генерация подписи
    //    var signature = GenerateSignature(
    //        secret: _apiSecret,
    //        timestamp: timestamp,
    //        method: method.Method,
    //        pathAndQuery: uri.PathAndQuery
    //    );

    //    // Кодирование passphrase
    //    var encodedPassphrase = EncodePassphrase(_apiSecret, _apiPassphrase);

    //    // Добавляем заголовки
    //    request.Headers.Add("KC-API-KEY", _apiKey);
    //    request.Headers.Add("KC-API-SIGN", signature);
    //    request.Headers.Add("KC-API-TIMESTAMP", timestamp);
    //    request.Headers.Add("KC-API-PASSPHRASE", encodedPassphrase);
    //    request.Headers.Add("KC-API-KEY-VERSION", "2");

    //    return request;
    //}
    //private static string GenerateSignature(string secret, string timestamp, string method, string pathAndQuery)
    //{
    //    var message = timestamp + method.ToUpper() + pathAndQuery;
    //    var keyBytes = Encoding.UTF8.GetBytes(secret);
    //    var messageBytes = Encoding.UTF8.GetBytes(message);

    //    using (var hmac = new HMACSHA256(keyBytes))
    //    {
    //        var hash = hmac.ComputeHash(messageBytes);
    //        return Convert.ToBase64String(hash);
    //    }
    //}
    //private static string EncodePassphrase(string secret, string passphrase)
    //{
    //    var keyBytes = Encoding.UTF8.GetBytes(secret);
    //    var passphraseBytes = Encoding.UTF8.GetBytes(passphrase);

    //    using (var hmac = new HMACSHA256(keyBytes))
    //    {
    //        var hash = hmac.ComputeHash(passphraseBytes);
    //        return Convert.ToBase64String(hash);
    //    }
    //}
}