using Arbitrage.ExchangeDomain.Enums;
using Arbitrage.Symbols.Presentation.Dto.CurrencyPair;
using DataSocketService.Exchanges.Base;
using DataSocketService.Other;
using Newtonsoft.Json.Linq;
using System.Text.Json;

public class KuCoinSocket : SocketBase
{
    protected override string urlFutures => "/contractMarket/level2Depth5";
    protected override string urlSpot => "/spotMarket/level2Depth5";

    public async override Task ConnectAsync()
    {
        (string endpoint, string token) = await GetWebSocketTokenAsync();
        if (endpoint == null)
        {
            Console.WriteLine("включи VPN");
            return;
        }
        var uri = new Uri($"{endpoint}?token={token}");
        await _webSocket.ConnectAsync(uri, CancellationToken.None);

        await base.ConnectAsync();
    }
    async Task<(string, string)> GetWebSocketTokenAsync()
    {
        try
        {
            using var httpClient = new HttpClient();
            var response = await httpClient.PostAsync("https://api.kucoin.com/api/v1/bullet-public", null);
            var content = await response.Content.ReadAsStringAsync();
            var dataJson = JObject.Parse(content)["data"];
            var token = dataJson["token"].ToString();
            var endpoint = dataJson["instanceServers"][0]["endpoint"].ToString();
            return (endpoint, token);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[{Name}] {ex.Message}");
            return (null, null);
        }
    }

    protected override (ParseObject, object) ParseMessage(ref Utf8JsonReader reader)
    {
        string ticker = string.Empty;
        DateTime time = DateTime.Now;
        List<(decimal, decimal)> bids = null;
        List<(decimal, decimal)> asks = null;

        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.PropertyName)
            {
                if (reader.ValueTextEquals("type"))
                {
                    reader.Read();
                    string value = reader.GetString();
                    if (ParsePong(value))
                    {
                        return (ParseObject.Pong, null);
                    }
                    //Console.WriteLine(reader.GetString());
                }
                else if (reader.ValueTextEquals("topic"))
                {
                    reader.Read();
                    string str = reader.GetString();
                    ticker = str.Split(":")[1];
                    //Console.WriteLine(reader.GetString());
                }
                else if (reader.ValueTextEquals("asks"))
                {
                    reader.Read();
                    asks = ReadArray(ref reader);
                }
                else if (reader.ValueTextEquals("bids"))
                {
                    reader.Read();
                    bids = ReadArray(ref reader);
                }
                else if (reader.ValueTextEquals("timestamp"))
                {
                    reader.Read();
                    time = DateTimeOffset.FromUnixTimeMilliseconds(reader.GetInt64()).ToLocalTime().DateTime;
                }
            }
        }
        if (string.IsNullOrEmpty(ticker))
            return (ParseObject.Other, null);

        var crypto = currencyPairDict[ticker];
        var book = crypto.Book;

        var multi = 1;// crypto.Info.Coin.Multiplier;
        book.Bids = bids.Select(bid => (bid.Item1, bid.Item2 * multi)).ToList();
        book.Asks = asks.Select(ask => (ask.Item1, ask.Item2 * multi)).ToList();
        book.ChangeDt(time);

        return (ParseObject.Data, null);
    }

    List<(decimal, decimal)> ReadArray(ref Utf8JsonReader reader)
    {
        List<(decimal, decimal)> dict = new();

        while (reader.Read() && reader.TokenType == JsonTokenType.StartArray)
        {
            reader.Read(); // price
            decimal price = FormatUtils.ToDec(reader.GetString());
            reader.Read(); // volume
            decimal volume = 0;
            if (AssetType == MarketType.Spot)
                volume = FormatUtils.ToDec(reader.GetString());
            else
                volume = reader.GetDecimal();
            reader.Read(); // EndArray
            dict.Add((price, volume));
        }
        return dict;
    }

    protected override string MessageSubscribe(CurrencyPairResponceDto info)
        => info.Ticker;

    protected override object MessageSubscribeAll(string[] messages)
    {
        return new
        {
            id = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString(),
            type = "subscribe",
            topic = $"{url}:{string.Join(",", messages)}",
            response = true
        };
    }
    protected override object MessagePing()
    {
        return new
        {
            id = Guid.NewGuid().ToString(),
            type = "ping"
        };
    }

    protected override bool ParsePong(string value)
    {
        string key = "pong";
        return value == key;
    }
}