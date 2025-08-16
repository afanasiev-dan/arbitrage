using Arbitrage.Core.Base.Enums;
using Arbitrage.Other;
using Arbitrage.Service.Base;
using System.Text.Json;

namespace Arbitrage.Service.Mexc
{
    internal class MexcSocket : SocketBook
    {
        protected override string urlFutures => "wss://contract.mexc.com/edge";

        protected override string urlSpot => "wss://wbs.mexc.com/ws";

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
            List<(decimal, decimal)> asks = new();
            List<(decimal, decimal)> bids = new();

            var keys = new
            {
                type = Settings.AssetType == AssetTypeEnum.Spot ? "msg" : "channel",
                ticker = Settings.AssetType == AssetTypeEnum.Spot ? "s" : "symbol",
                data = Settings.AssetType == AssetTypeEnum.Spot ? "d" : "data",
                time = Settings.AssetType == AssetTypeEnum.Spot ? "t" : "ts"
            };
            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.PropertyName)
                {
                    if (reader.ValueTextEquals(keys.type))
                    {
                        reader.Read();
                        string value = reader.GetString();
                        if (ParsePong(value))
                        {
                            return (ParseObject.Pong, null);
                        }
                        //Console.WriteLine(reader.GetString());
                    }
                    else if (reader.ValueTextEquals(keys.ticker))
                    {
                        reader.Read();
                        ticker = reader.GetString();
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
                    else if (reader.ValueTextEquals(keys.time))
                    {
                        reader.Read();
                        time = DateTimeOffset.FromUnixTimeMilliseconds(reader.GetInt64()).ToLocalTime().DateTime;
                    }
                }
            }
            if (string.IsNullOrEmpty(ticker))
                return (ParseObject.Other, null);

            var coin = updateDict[ticker];
            var book = coin.Book;
            book.Bids = bids;
            book.Asks = asks;
            book.ChangeDt(time, coin.LogPrint());

            return (ParseObject.Data, null);
        }

        List<(decimal, decimal)> ReadArray(ref Utf8JsonReader reader)
        {
            List<(decimal, decimal)> dict = new();
            if (Settings.AssetType == AssetTypeEnum.Spot)
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
                            else if (reader.ValueTextEquals("v"))
                            {
                                reader.Read();
                                volume = F.ToDec(reader.GetString());
                            }
                        }
                    }
                    dict.Add((price, volume));
                }
            }
            else
            {
                while (reader.Read() && reader.TokenType == JsonTokenType.StartArray)
                {
                    reader.Read(); // price
                    decimal price = reader.GetDecimal();
                    reader.Read(); // volume
                    decimal volume = reader.GetDecimal();
                    reader.Read(); // skip num
                    reader.Read(); // EndArray
                    dict.Add((price, volume));
                }
            }
            return dict;
        }

        protected override string MessageSubscribe(CoinInfo coin)
         => Settings.AssetType == AssetTypeEnum.Spot ? $"spot@public.limit.depth.v3.api@{coin.Ticker}@5" : coin.Ticker;

        protected override object MessageSubscribeAll(string[] messages)
        {
            if (Settings.AssetType == AssetTypeEnum.Spot)
            {
                return new
                {
                    method = "SUBSCRIPTION",
                    @params = messages
                };
            }
            else
            {
                return new
                {
                    method = "sub.depth.full",
                    param = new
                    {
                        symbol = string.Join("", messages),
                        limit = 5
                    }
                };
            }
        }

        protected override object MessagePing()
        {
            return new
            {
                method = Settings.AssetType == AssetTypeEnum.Spot ? "PING" : "ping"
            };
        }

        protected override bool ParsePong(string value)
        {
            return value.ToLower() == "pong";
        }
    }
}
