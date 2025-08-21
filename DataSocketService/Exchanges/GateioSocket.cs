using Arbitrage.ExchangeDomain.Enums;
using Arbitrage.Symbols.Presentation.Dto.CurrencyPair;
using DataSocketService.Exchanges.Base;
using DataSocketService.Other;
using System.Text.Json;

namespace Arbitrage.Service.Gateio
{
    internal class GateioSocket : SocketBase
    {
        protected override string urlSpot => "wss://api.gateio.ws/ws/v4/";
        protected override string urlFutures => "wss://fx-ws.gateio.ws/v4/ws/usdt";

        public override async Task ConnectAsync()
        {
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
                    else if (reader.ValueTextEquals(AssetType == MarketType.Spot ? "s" : "contract"))
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

            var crypto = currencyPairDict[ticker];
            var book = crypto.Book;

            //var multi = crypto.Info.Coin.Multiplier;
            var multi = 1;
            book.Bids = bids.Select(bid => (bid.Item1, bid.Item2 * multi)).ToList();
            book.Asks = asks.Select(ask => (ask.Item1, ask.Item2 * multi)).ToList();
            book.ChangeDt(time);

            return (ParseObject.Data, null);
        }

        List<(decimal, decimal)> ReadArray(ref Utf8JsonReader reader)
        {
            List<(decimal, decimal)> dict = new();
            if (AssetType == MarketType.Spot)
            {
                while (reader.Read() && reader.TokenType == JsonTokenType.StartArray)
                {
                    reader.Read(); // price
                    decimal price = FormatUtils.ToDec(reader.GetString());
                    reader.Read(); // volume
                    decimal volume = FormatUtils.ToDec(reader.GetString());
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
                                price = FormatUtils.ToDec(reader.GetString());
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

        protected override string MessageSubscribe(CurrencyPairResponceDto info)
        {
            return info.Ticker;
        }
        protected override object MessageSubscribeAll(string[] messages)
        {
            if (AssetType == MarketType.Spot)
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
                channel = AssetType == MarketType.Spot ? "spot.ping" : "futures.ping"
            };
        }

        protected override bool ParsePong(string value)
        {
            string key = AssetType == MarketType.Spot ? "spot.pong" : "futures.pong";
            return value == key;
        }

    }
}
