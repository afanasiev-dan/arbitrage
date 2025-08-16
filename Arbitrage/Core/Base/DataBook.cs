using Arbitrage.Core.Base.Enums;
using Arbitrage.Service.Base;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace Arbitrage.Core.Base
{
    public abstract class DataBook
    {
        protected ExchangeAssetInfo Settings;
        protected string url => Settings.AssetType == AssetTypeEnum.Spot ? urlSpot : urlFutures;
        protected abstract string urlFutures { get; }
        protected abstract string urlSpot { get; }

        public ConcurrentDictionary<string, Crypto> updateDict = new();
        protected string id;

        public virtual void Init(ExchangeAssetInfo settings, string id)
        {
            Settings = settings;
            this.id = id;
        }
        public virtual bool AddToSocket(Crypto crypto)
        {
            var ticker = crypto.Info.Coin.Ticker.ToUpper();
            updateDict[ticker] = crypto;

            return true;
        }
    }
}
