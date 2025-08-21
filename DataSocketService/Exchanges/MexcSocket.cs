using Arbitrage.ExchangeDomain.Enums;
using Arbitrage.Symbols.Presentation.Dto.CurrencyPair;
using DataSocketService.Exchanges.Base;
using DataSocketService.Other;
using System.Text.Json;

namespace Arbitrage.Service.Mexc
{
    internal class MexcSocket : SocketBase
    {
        protected override string urlFutures => "wss://contract.mexc.com/edge";

        protected override string urlSpot => "wss://wbs.mexc.com/ws";

        public override async Task ConnectAsync()
        {
            string url = AssetType == MarketType.Spot ? urlSpot : urlFutures;
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
                type = AssetType == MarketType.Spot ? "msg" : "channel",
                ticker = AssetType == MarketType.Spot ? "s" : "symbol",
                data = AssetType == MarketType.Spot ? "d" : "data",
                time = AssetType == MarketType.Spot ? "t" : "ts"
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

            var coin = currencyPairDict[ticker];
            var book = coin.Book;
            book.Bids = bids;
            book.Asks = asks;
            book.ChangeDt(time);

            return (ParseObject.Data, null);
        }

        List<(decimal, decimal)> ReadArray(ref Utf8JsonReader reader)
        {
            List<(decimal, decimal)> dict = new();
            if (AssetType == MarketType.Spot)
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
                            else if (reader.ValueTextEquals("v"))
                            {
                                reader.Read();
                                volume = FormatUtils.ToDec(reader.GetString());
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

        protected override string MessageSubscribe(CurrencyPairResponceDto info)
         => AssetType == MarketType.Spot ? $"spot@public.limit.depth.v3.api@{info.Ticker}@5" : info.Ticker;

        protected override object MessageSubscribeAll(string[] messages)
        {
            if (AssetType == MarketType.Spot)
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
                method = AssetType == MarketType.Spot ? "PING" : "ping"
            };
        }

        protected override bool ParsePong(string value)
        {
            return value.ToLower() == "pong";
        }
    }
}
