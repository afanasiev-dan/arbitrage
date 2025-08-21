using Arbitrage.Core.Base.Enums;
using Arbitrage.Other;
using Arbitrage.Service.Base;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace Arbitrage.Service.ByBit
{
    public class ByBitSocket : SocketBook
    {
        protected override string urlFutures => "wss://stream.bybit.com/v5/public/linear";
        protected override string urlSpot => "wss://stream.bybit.com/v5/public/spot";

        public async override Task ConnectAsync()
        {
            var uri = new Uri(url);
            await _webSocket.ConnectAsync(uri, CancellationToken.None);

            await base.ConnectAsync();
        }

        protected override (ParseObject, object) ParseMessage(ref Utf8JsonReader reader)
        {
            string ticker = string.Empty;
            DateTime time = DateTime.Now;
            List<(decimal, decimal)> bids = new();
            List<(decimal, decimal)> asks = new();
            string type = string.Empty;

            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.PropertyName)
                {
                    if (reader.ValueTextEquals("ret_msg"))
                    {
                        reader.Read();
                        string value = reader.GetString();
                        if (ParsePong(value))
                        {
                            return (ParseObject.Pong, null);
                        }
                    }
                    else if (reader.ValueTextEquals("type"))
                    {
                        reader.Read();
                        type = reader.GetString();
                    }
                    else if (reader.ValueTextEquals("s"))
                    {
                        reader.Read();
                        ticker = reader.GetString();
                    }
                    else if (reader.ValueTextEquals("ts"))
                    {
                        reader.Read();
                        time = DateTimeOffset.FromUnixTimeMilliseconds(reader.GetInt64()).ToLocalTime().DateTime;
                    }
                    else if (reader.ValueTextEquals("a"))
                    {
                        reader.Read();
                        asks = ReadArray(ref reader);
                    }
                    else if (reader.ValueTextEquals("b"))
                    {
                        reader.Read();
                        bids = ReadArray(ref reader);
                    }
                }
            }

            if (string.IsNullOrEmpty(ticker))
                return (ParseObject.Other, null);

            var coin = updateDict[ticker];
            var book = coin.Book;
            if (type == "snapshot")
            {
                book.Asks.Clear();
                book.Bids.Clear();
                UpdateSide(book.Asks, asks);
                UpdateSide(book.Bids, bids);
            }
            else if (type == "delta")
            {
                UpdateSide(book.Asks, asks);
                UpdateSide(book.Bids, bids);
            }
            book.ChangeDt(time, coin.LogPrint());

            return (ParseObject.Data, null);
        }
        List<(decimal,decimal)> ReadArray(ref Utf8JsonReader reader)
        {
            List<(decimal,decimal)> dict = new();
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

        private void UpdateSide(List<(decimal price, decimal volume)> side, List<(decimal price, decimal volume)> updates)
        {
            if (updates == null) return;

            foreach (var update in updates)
            {
                int index = side.FindIndex(x => x.price == update.price);

                if (update.volume == 0)
                {
                    if (index != -1)
                        side.RemoveAt(index);
                }
                else
                {
                    if (index != -1)
                        side[index] = update;
                    else
                        side.Add(update);
                }
            }
        }


        protected override string MessageSubscribe(CoinInfo coin)
        {
            return $"orderbook.50.{coin.Ticker}";
        }
        protected override object MessageSubscribeAll(string[] messages)
        {
            return new
            {
                op = "subscribe",
                args = messages
            };
        }

        protected override bool ParsePong(string value)
        {
            string key = "pong";
            return value == key;
        }
    }
}
