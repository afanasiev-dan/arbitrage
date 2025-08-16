using Arbitrage.Core.Base.Enums;
using Arbitrage.Other;
using Arbitrage.Service.Base;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Arbitrage.Service.LBank
{
    internal class LBankSocketV1 : SocketBook
    {
        protected override string urlFutures => throw new NotImplementedException();

        protected override string urlSpot => "wss://www.lbkex.net/ws/V2/";

        public async override Task ConnectAsync()
        {
            string url = Settings.AssetType == AssetTypeEnum.Spot ? urlSpot : urlFutures;
            var uri = new Uri(url);
            await _webSocket.ConnectAsync(uri, CancellationToken.None);
            await base.ConnectAsync();
        }

        //protected override bool ParseMessage(string ticker, JsonElement root)
        //{
        //    bool succes = false;
        //    try
        //    {
        //        if (Settings.AssetType == AssetTypeEnum.Spot)
        //            succes = ParseSpot(ticker, root);
        //        else
        //            ParseFutures(root);
        //        return succes;
        //    }
        //    catch (Exception ex)
        //    {
        //    }
        //    return false;
        //}
        //bool ParseSpot(string ticker, JsonElement root)
        //{
        //    if (!root.TryGetProperty("type", out var typeElement) || typeElement.GetString() != "depth")
        //        return false;
        //    if (!root.TryGetProperty("depth", out var depthElement))
        //        return false;

        //    var asks = ParsePriceLevels(depthElement, "asks");
        //    var bids = ParsePriceLevels(depthElement, "bids");

        //    if (updateDict.TryGetValue(ticker, out var coin))
        //    {
        //        var book = coin.Book;
        //        book.Asks = asks;
        //        book.Bids = bids;

        //        if (root.TryGetProperty("TS", out var timestampElement))
        //        {
        //            string ts = timestampElement.GetString();
        //            var dt = DateTime.Parse(ts).AddHours(-5);
        //            book.ChangeDt(dt, coin.LogPrint());
        //        }
        //    }

        //    return true;
        //}
        private SortedDictionary<decimal, decimal> ParsePriceLevels(JsonElement dataElement, string side)
        {
            SortedDictionary<decimal, decimal> levels = side == "asks" ? new() : new(new DescComparer());
            if (dataElement.TryGetProperty(side, out var sideElement))
            {
                foreach (var level in sideElement.EnumerateArray())
                {
                    if (level.GetArrayLength() >= 2)
                    {
                        levels.Add(
                            F.ToDec(level[0].GetString()),
                            F.ToDec(level[1].GetString())
                        );
                    }
                }
            }
            return levels;
        }

        void ParseFutures(JsonElement jObj)
        {
            throw new NotImplementedException();
        }

        protected override string MessageSubscribe(CoinInfo coin)
        => coin.Ticker;

        protected override object MessageSubscribeAll(string[] messages)
        {
            if (Settings.AssetType == AssetTypeEnum.Spot)
            {
                return new
                {
                    action = "subscribe",
                    subscribe = "depth",
                    depth = 10,
                    pair = string.Join("", messages),
                };
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        //protected override string GetTicker(JsonElement root)
        //{
        //    if (Settings.AssetType == AssetTypeEnum.Spot)
        //    {
        //        if (root.TryGetProperty("pair", out var topicElement))
        //            return topicElement.ToString();
        //        return string.Empty;
        //    }
        //    else
        //    {
        //        return string.Empty;
        //    }
        //}


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

        //protected override bool ParsePong(JsonElement root)
        //{
        //    return root.TryGetProperty("pong", out var tsElemPong);
        //}

        protected override object MessagePing()
        {
            var id = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString();
            return new
            {
                action = "ping",
                ping = id
            };
        }

        protected override bool ParsePong(string value)
        {
            throw new NotImplementedException();
        }

        protected override (ParseObject, object) ParseMessage(ref Utf8JsonReader reader)
        {
            throw new NotImplementedException();
        }
    }
}
