using Arbitrage.Core.Base.Enums;
using Arbitrage.Service.Base;
using System.Text.Json;

namespace Arbitrage.Service.HTX
{
    internal class HTXSocket : SocketBook
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
            string url = Settings.AssetType == AssetTypeEnum.Spot ? urlSpot : urlFutures;
            var uri = new Uri(url);
            await _webSocket.ConnectAsync(uri, CancellationToken.None);

            await base.ConnectAsync();
        }

        protected override (ParseObject, object) ParseMessage(ref Utf8JsonReader reader)
        {
            Crypto crypto = null;
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
                        crypto = updateDict[ticker.ToUpper()];
                        //Console.WriteLine(reader.GetString());
                    }
                    else if (reader.ValueTextEquals(AsksProp.EncodedUtf8Bytes))
                    {
                        reader.Read();
                        asks = ReadArray(ref reader, crypto.Info.Coin.Multiplier);
                    }
                    else if (reader.ValueTextEquals(BidsProp.EncodedUtf8Bytes))
                    {
                        reader.Read();
                        bids = ReadArray(ref reader, crypto.Info.Coin.Multiplier);
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
            var multi = crypto.Info.Coin.Multiplier;
            book.Bids = bids;
            book.Asks = asks;
            book.ChangeDt(time, crypto.LogPrint());

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

        protected override string MessageSubscribe(CoinInfo coin)
        {
            return Settings.AssetType == AssetTypeEnum.Spot ?
                         $"market.{coin.Ticker.ToLower()}.mbp.refresh.5" :
                         $"market.{coin.Ticker}.depth.step6";
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
