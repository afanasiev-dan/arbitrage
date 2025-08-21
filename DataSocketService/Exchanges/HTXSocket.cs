using Arbitrage.ExchangeDomain.Enums;
using Arbitrage.Symbols.Presentation.Dto.CurrencyPair;
using DataSocketService;
using DataSocketService.Exchanges.Base;
using DataSocketService.Model;
using System.Text.Json;

namespace Arbitrage.Service.HTX
{
    internal class HTXSocket : SocketBase
    {
        protected override string urlFutures => "wss://api.hbdm.com/linear-swap-ws";
        protected override string urlSpot => "wss://api-aws.huobi.pro/ws";

        static readonly JsonEncodedText ChProp = JsonEncodedText.Encode("ch");
        static readonly JsonEncodedText PingProp = JsonEncodedText.Encode("ping");
        static readonly JsonEncodedText AsksProp = JsonEncodedText.Encode("asks");
        static readonly JsonEncodedText BidsProp = JsonEncodedText.Encode("bids");
        static readonly JsonEncodedText TsProp = JsonEncodedText.Encode("ts");

        public async override Task ConnectAsync()
        {
            string url = AssetType == MarketType.Spot ? urlSpot : urlFutures;
            var uri = new Uri(url);
            await _webSocket.ConnectAsync(uri, CancellationToken.None);

            await base.ConnectAsync();
        }

        protected override (ParseObject, object) ParseMessage(ref Utf8JsonReader reader)
        {
            CurrencyPairBook crypto = null;
            DateTime time = DateTime.Now;
            List<(decimal, decimal)> asks = null;
            List<(decimal, decimal)> bids = null;

            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.PropertyName)
                {
                    if (reader.ValueTextEquals(PingProp.EncodedUtf8Bytes))
                    {
                        reader.Read();
                        var value = reader.GetInt64();
                        return (ParseObject.Ping, value);
                        //Console.WriteLine(reader.GetString());
                    }
                    else if (reader.ValueTextEquals(ChProp.EncodedUtf8Bytes) && crypto == null)
                    {
                        reader.Read();
                        string str = reader.GetString();
                        string ticker = str.Split(".")[1];
                        crypto = currencyPairDict[ticker];
                        //Console.WriteLine(reader.GetString());
                    }
                    else if (reader.ValueTextEquals(AsksProp.EncodedUtf8Bytes))
                    {
                        reader.Read();
                        asks = ReadArray(ref reader, 1);//crypto.Info.Coin.Multiplier
                    }
                    else if (reader.ValueTextEquals(BidsProp.EncodedUtf8Bytes))
                    {
                        reader.Read();
                        bids = ReadArray(ref reader, 1);//crypto.Info.Coin.Multiplier
                    }
                    else if (reader.ValueTextEquals(TsProp.EncodedUtf8Bytes))
                    {
                        reader.Read();
                        time = DateTimeOffset.FromUnixTimeMilliseconds(reader.GetInt64()).ToLocalTime().DateTime;
                    }
                }
            }
            if (crypto == null)
                return (ParseObject.Other, null);

            var book = crypto.Book;
            var multi = 1;//crypto.Info.Coin.Multiplier crypto.Info.Coin.Multiplier;
            book.Bids = bids;
            book.Asks = asks;
            book.ChangeDt(time);

            return (ParseObject.Data, null);
        }

        List<(decimal, decimal)> ReadArray(ref Utf8JsonReader reader, decimal multi)
        {
            List<(decimal,decimal)> dict = new();
            int count = 0;
            while (reader.Read() && reader.TokenType == JsonTokenType.StartArray && count < LaunchConfig.depthGlass)
            {
                reader.Read(); // price
                decimal price = reader.GetDecimal();
                reader.Read(); // volume
                decimal volume = reader.GetDecimal() * multi;
                reader.Read(); // EndArray
                dict.Add((price, volume));
                count++;
            }
            while (reader.TokenType != JsonTokenType.EndArray)
                reader.Read();
            return dict;
        }

        protected override string MessageSubscribe(CurrencyPairResponceDto info)
        {
            return AssetType == MarketType.Spot ?
                         $"market.{info.Ticker.ToLower()}.mbp.refresh.5" :
                         $"market.{info.Ticker}.depth.step6";
        }

        protected override object MessageSubscribeAll(string[] messages)
        {
            return new
            {
                sub = string.Join("", messages),
                id = Guid.NewGuid().ToString()
            };
        }

        protected override bool ParsePong(string value)
        {
            throw new NotImplementedException();
        }
    }
}
