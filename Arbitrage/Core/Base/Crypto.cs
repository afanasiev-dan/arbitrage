using Arbitrage.Core.Base;
using Arbitrage.Core.Base.Enums;
using Arbitrage.Core.Other;
using Arbitrage.Other;
using System.Text.Json;
using System.Threading.Channels;

namespace Arbitrage.Service.Base
{
    public class Crypto
    {
        public CryptoAssetInfo Info;
        public OrderBookData Book = new();
        public FundingData Funding = new();
        public Exchange Exchange;
        public Dictionary<DateTime, decimal> Prices = new();

        public async Task Create(Exchange exchange, CryptoAssetInfo info)
        {
            Exchange = exchange;
            Info = info;
            await Exchange.SubscribeBookUpdates(this);

            //Console.WriteLine($"Create {info.Str()}");
        }

        public bool SuccesBook()
        {
            if(Book == null) return false;
            return Book.isSuccess;
        }

        public string LogPrint()
        {
            return $"[{Exchange.Settings.Print()}#{Info.Coin.BaseCoin}]";
        }

        public string Print()
        {
            string msg = Info.Print() + $" {Exchange.Settings.Name} ";
            if (Book?.Asks.Count > 0)
                msg += $" {Book.Ask}";
            if (Book?.Bids.Count > 0)
                msg += $" {Book.Bid}";
            msg += $" | sec: {(DateTime.Now - Book?.dt).Value.TotalSeconds}";
            return msg;
        }   
        public string PrintNice()
        {
            string msg = $"[{Exchange.Settings.Print()}]({Network.GetUrl(Info.Coin.BaseCoin, Exchange.Settings.Name, Exchange.Settings.AssetType)})\n";

            if (Book?.Asks.Count > 0)
                msg += $"Ask: {Book.Ask}\n";
            if (Book?.Bids.Count > 0)
                msg += $"Bid: {Book.Bid}\n";
            if(Exchange.Settings.AssetType == AssetTypeEnum.Futures)
                msg += $"{Funding.Print()}\n";
            msg += $"LUpdate: {(DateTime.Now - Book?.dt).Value.TotalSeconds}\n";
            return msg;
        }
    }

    public class ExchangeAssetInfo
    {
        public ExchangeEnum Name;
        public AssetTypeEnum AssetType;
        public SocketSettings Socket;
        public int LimitResponse;

        public ExchangeAssetInfo(ExchangeEnum name, AssetTypeEnum assetType, int limitResponse = 0, SocketSettings socket = null)
        {
            Name = name;
            AssetType = assetType;
            LimitResponse = limitResponse;
            Socket = socket;
        }

        public string Print() => StringHelper.ExchangeStr(Name, AssetType);
    }

    public class SocketSettings
    {
        public int WsCap;
        public int MaxSub;

        public bool CheckConnectByPing;
        public float TimerWaitPong;
        public int IntervalPing;

        public SocketSettings(int wsCap = int.MaxValue, int maxSub = 0, bool checkConnectByPing = true, float timerWaitPong = 0, int intervalPing = 0)
        {
            WsCap = wsCap;
            CheckConnectByPing = checkConnectByPing;

            if (maxSub == 0)
                MaxSub = wsCap;
            else
                MaxSub = maxSub;
            if (timerWaitPong == 0)
                TimerWaitPong = LaunchConfig.TimerWaitPong;
            else
                TimerWaitPong = timerWaitPong;

            if (intervalPing == 0)
                IntervalPing = LaunchConfig.IntervalPing;
            else
                IntervalPing = intervalPing;

            TimerWaitPong *= LaunchConfig.xWait;
        }
    }

    public class CryptoAssetInfo
    {
        public CoinInfo Coin;
        public AssetTypeEnum AssetType;

        public CryptoAssetInfo(CoinInfo coin, AssetTypeEnum type)
        {
            Coin = coin;
            AssetType = type;
        }

        public string Print()
        {
            return $"{Coin.BaseCoin} {AssetType}";
        }
    }
}
