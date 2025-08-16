using Arbitrage.Core.Base.Enums;
using Arbitrage.Other;
using Arbitrage.Service.Base;
using System.Text.Json;
using Telegram.Bot.Types;

namespace Arbitrage.Service.Gateio
{
    internal class GateioSocket : SocketBook
    {
        protected override string urlSpot => "wss://api.gateio.ws/ws/v4/";
        protected override string urlFutures => "wss://fx-ws.gateio.ws/v4/ws/usdt";

        public override async Task ConnectAsync()
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
            List<(decimal, decimal)> bids = null;
            List<(decimal, decimal)> asks = null;

            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.PropertyName)
                {
                    if (reader.ValueTextEquals("channel"))
                    {
                        reader.Read();
                        string value = reader.GetString();
                        if (ParsePong(value))
                        {
                            return (ParseObject.Pong, null);
                        }
                        //Console.WriteLine(reader.GetString());
                    }
                    else if (reader.ValueTextEquals(Settings.AssetType == AssetTypeEnum.Spot ? "s" : "contract"))
                    {
                        reader.Read();
                        ticker = reader.GetString();
                        //Console.WriteLine(reader.GetString());
                    }
                    else if (reader.ValueTextEquals("bids"))
                    {
                        reader.Read();
                        bids = ReadArray(ref reader);
                    }
                    else if (reader.ValueTextEquals("asks"))
                    {
                        reader.Read();
                        asks = ReadArray(ref reader);
                    }
                    else if (reader.ValueTextEquals("t"))
                    {
                        reader.Read();
                        time = DateTimeOffset.FromUnixTimeMilliseconds(reader.GetInt64()).ToLocalTime().DateTime;
                    }
                }
            }
            if (string.IsNullOrEmpty(ticker))
                return (ParseObject.Other, null);

            var crypto = updateDict[ticker];
            var book = crypto.Book;

            var multi = crypto.Info.Coin.Multiplier;
            book.Bids = bids.Select(bid => (bid.Item1, bid.Item2 * multi)).ToList();
            book.Asks = asks.Select(ask => (ask.Item1, ask.Item2 * multi)).ToList();
            book.ChangeDt(time, crypto.LogPrint());

            return (ParseObject.Data, null);
        }

        List<(decimal, decimal)> ReadArray(ref Utf8JsonReader reader)
        {
            List<(decimal, decimal)> dict = new();
            if (Settings.AssetType == AssetTypeEnum.Spot)
            {
                while (reader.Read() && reader.TokenType == JsonTokenType.StartArray)
                {
                    reader.Read(); // price
                    decimal price = F.ToDec(reader.GetString());
                    reader.Read(); // volume
                    decimal volume = F.ToDec(reader.GetString());
                    reader.Read(); // EndArray
                    dict.Add((price, volume));
                }
            }
            else
            {
                while (reader.Read() && reader.TokenType == JsonTokenType.StartObject)
                {
                    decimal price = 0;
                    decimal volume = 0;
                    while (reader.Read() && reader.TokenType != JsonTokenType.EndObject)
                    {
                        if (reader.TokenType == JsonTokenType.PropertyName)
                        {
                            if (reader.ValueTextEquals("p"))
                            {
                                reader.Read();
                                price = F.ToDec(reader.GetString());
                            }
                            else if (reader.ValueTextEquals("s"))
                            {
                                reader.Read();
                                volume = reader.GetDecimal();
                            }
                        }
                    }
                    dict.Add((price, volume));
                }
            }
            return dict;
        }

        protected override string MessageSubscribe(CoinInfo coin)
        {
            return coin.Ticker;
        }
        protected override object MessageSubscribeAll(string[] messages)
        {
            if (Settings.AssetType == AssetTypeEnum.Spot)
            {
                return new
                {
                    time = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
                    channel = "spot.order_book",
                    @event = "subscribe",
                    payload = new object[] { string.Join("", messages), "5", "1000ms" }
                };
            }
            else
            {
                return new
                {
                    time = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
                    channel = "futures.order_book",
                    @event = "subscribe",
                    payload = new object[] { string.Join("", messages), "5", "0" }
                };
            }
        }
        protected override object MessagePing()
        {
            return new
            {
                //time = $"{DateTimeOffset.UtcNow.ToUnixTimeSeconds()}",
                channel = Settings.AssetType == AssetTypeEnum.Spot ? "spot.ping" : "futures.ping"
            };
        }

        protected override bool ParsePong(string value)
        {
            string key = Settings.AssetType == AssetTypeEnum.Spot ? "spot.pong" : "futures.pong";
            return value == key;
        }

    }
}
