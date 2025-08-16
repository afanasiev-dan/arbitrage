using Arbitrage.Core.Base.Enums;
using Arbitrage.Other;
using Arbitrage.Service.Base;
using System.Text.Json;

namespace Arbitrage.Service.LBank
{
    internal class LBankSocketV2 : SocketBook
    {
        protected override string urlFutures => "wss://uuws.ierpifvid.com/ws/v3";
        protected override string urlSpot => "wss://ccws.ierpifvid.com/ws/V3/";
        static readonly JsonEncodedText ActionProp = JsonEncodedText.Encode("action");
        static readonly JsonEncodedText PairProp = JsonEncodedText.Encode("pair");
        static readonly JsonEncodedText AsksProp = JsonEncodedText.Encode("asks");
        static readonly JsonEncodedText BidsProp = JsonEncodedText.Encode("bids");
        static readonly JsonEncodedText TsProp = JsonEncodedText.Encode("TS");

        public async override Task ConnectAsync()
        {
            string url = Settings.AssetType == AssetTypeEnum.Spot ? urlSpot : urlFutures;
            var uri = new Uri(url);
            await _webSocket.ConnectAsync(uri, CancellationToken.None);
            await base.ConnectAsync();
        }

        protected override (ParseObject, object) ParseMessage(ref Utf8JsonReader reader)
        {
            string ticker = string.Empty;
            DateTime time = DateTime.Now;
            List<(decimal, decimal)> asks = null;
            List<(decimal, decimal)> bids = null;

            while (reader.Read())
            {
                if (reader.TokenType != JsonTokenType.PropertyName) continue;

                if (reader.ValueTextEquals(ActionProp.EncodedUtf8Bytes))
                {
                    reader.Read();
                    var value = reader.GetString();
                    if (ParsePong(value))
                        return (ParseObject.Pong, null);
                    //Console.WriteLine(reader.GetString());
                }
                else if (reader.ValueTextEquals(PairProp.EncodedUtf8Bytes))
                {
                    reader.Read();
                    ticker = reader.GetString();
                    //Console.WriteLine(reader.GetString());
                }
                else if (reader.ValueTextEquals(AsksProp.EncodedUtf8Bytes))
                {
                    reader.Read();
                    asks = ReadArray(ref reader);
                }
                else if (reader.ValueTextEquals(BidsProp.EncodedUtf8Bytes))
                {
                    reader.Read();
                    bids = ReadArray(ref reader);
                }
                else if (reader.ValueTextEquals(TsProp.EncodedUtf8Bytes))
                {
                    reader.Read();
                    string ts = reader.GetString();
                    time = DateTime.Parse(ts).AddHours(-5);
                }
            }
            if (string.IsNullOrEmpty(ticker))
                return (ParseObject.Other, null);

            var coin = updateDict[ticker.ToUpper()];
            var book = coin.Book;
            book.Bids = bids;
            book.Asks = asks;
            book.ChangeDt(time, coin.LogPrint());

            return (ParseObject.Data, null);
        }
        List<(decimal, decimal)> ReadArray(ref Utf8JsonReader reader)
        {
            List<(decimal, decimal)> dict = new();
            while (reader.Read() && reader.TokenType == JsonTokenType.StartArray)
            {
                reader.Read(); // price
                decimal price = F.ToDec(reader.GetString());
                reader.Read(); // volume
                decimal volume = F.ToDec(reader.GetString());
                reader.Read(); // EndArray
                dict.Add((price, volume));
            }
            return dict;
        }

        //bool ParseFutures(string ticker, JsonElement root)
        //{
        //    var asks = ParsePriceLevels(root, "s");
        //    var bids = ParsePriceLevels(root, "b");

        //    if (updateDict.TryGetValue(ticker, out var coin))
        //    {
        //        var book = coin.Book;
        //        book.Asks = asks;
        //        book.Bids = bids;

        //        if (root.TryGetProperty("w", out var timestampElement))
        //        {
        //            long timestamp = timestampElement.GetInt64();
        //            book.ChangeDt(
        //                DateTimeOffset.FromUnixTimeMilliseconds(timestamp).ToLocalTime().DateTime,
        //                coin.LogPrint()
        //            );
        //            return true;
        //        }
        //    }

        //    return false;
        //}

        protected override string MessageSubscribe(CoinInfo coin)
        {
            if (Settings.AssetType == AssetTypeEnum.Spot)
                return coin.Ticker;
            else
                return $"{coin.Ticker.ToUpper()}_{coin.PriceTick.ToString().Replace(',', '.')}_25";
        }
        protected override object MessageSubscribeAll(string[] messages)
        {
            var tickerM = string.Join("", messages);
            var ticker = tickerM.Split('_')[0];
            if (Settings.AssetType == AssetTypeEnum.Spot)
            {
                //inc
                //return new
                //{
                //    dataType = 1,
                //    depth = 5,
                //    pair = string.Join("", messages),
                //    action = "subscribe",
                //    subscribe = "depth",
                //    msgType = 2,
                //    limit = 50,
                //    type = 100
                //};
                return new
                {
                    action = "subscribe",
                    depth = 5,
                    limit = 50,
                    pair = tickerM,
                    subscribe = "fdepth",
                    symbol = tickerM
                };
            }
            else
            {
                return new
                {
                    x = 3,
                    y = ticker,
                    a = new
                    {
                        i = $"{tickerM}",
                    },
                    z = 1
                };
            }
        }

        //protected override (bool Success, object Obj) ParsePing(JsonElement root)
        //{
        //    if (root.TryGetProperty("ping", out var tsElem))
        //    {
        //        var obj = new
        //        {
        //            action = "pong",
        //            pong = tsElem.GetString()
        //        };
        //        return (true, obj);
        //    }
        //    return (false, null);
        //}

        protected override object MessagePing()
        {
            if (Settings.AssetType == AssetTypeEnum.Spot)
            {
                var id = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString();
                return new
                {
                    action = "ping",
                    ping = id
                };
            }
            else
            {
                return "ping";
            }
        }

        protected override bool ParsePong(string value)
        {
            string key = "pong";
            return value == key;
        }
    }
}
