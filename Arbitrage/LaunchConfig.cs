using Arbitrage.Core.Base.Enums;
using Arbitrage.Service.Base;

namespace Arbitrage
{
    public class LaunchConfig
    {
        public const int depthGlass = 5;
        public const int limitRep = 20;
        public const int wsCap = 30;
        public const float xWait = 2;

        public static List<ExchangeAssetInfo> ExchangeAssets = new()
        {
            new(ExchangeEnum.Gateio, AssetTypeEnum.Spot, limitResponse:45 * 2, new SocketSettings(wsCap:wsCap, maxSub:1)),//amazon(high) 
            new(ExchangeEnum.Gateio, AssetTypeEnum.Futures,limitResponse:45 * 2, new SocketSettings(wsCap:wsCap, maxSub:1)), //amazon
           
            new(ExchangeEnum.ByBit, AssetTypeEnum.Spot,limitResponse:limitRep, new SocketSettings(wsCap:wsCap,  maxSub:1)), //hel
            new(ExchangeEnum.ByBit, AssetTypeEnum.Futures,limitResponse:limitRep, new SocketSettings(wsCap:wsCap)), //hel (high)
            
            new(ExchangeEnum.KuCoin, AssetTypeEnum.Spot, limitResponse:45*2, new SocketSettings(wsCap:wsCap)), //100 max wsCap (150mb)
            new(ExchangeEnum.KuCoin, AssetTypeEnum.Futures,limitResponse:45*2, new SocketSettings(wsCap: wsCap)), //100 max wsCap

            new(ExchangeEnum.Mexc, AssetTypeEnum.Spot, limitResponse: 20, new SocketSettings(wsCap:30,maxSub:15)), //30 max wsCap
            new(ExchangeEnum.Mexc, AssetTypeEnum.Futures, limitResponse: 100, new SocketSettings(wsCap:wsCap,  maxSub:1)), //76 sec

            new(ExchangeEnum.HTX, AssetTypeEnum.Spot, limitResponse:limitRep, new SocketSettings(wsCap:wsCap, maxSub:1, checkConnectByPing: false, timerWaitPong:20)), //30 sec
            new(ExchangeEnum.HTX, AssetTypeEnum.Futures,  limitResponse:limitRep, new SocketSettings(wsCap:10, maxSub:1, checkConnectByPing: false, timerWaitPong:20)), //8 sec

            new(ExchangeEnum.LBank, AssetTypeEnum.Spot,  limitResponse: 50, new SocketSettings(wsCap:50, maxSub:1)),
            //new(ExchangeEnum.LBank, AssetTypeEnum.Futures,  limitResponse: 50, new SocketSettings(wsCap:1, maxSub:1, intervalPing: 8)),
        };

        public const bool IsDebug = false;

        public const decimal FundingUpdate = 10; //min
        public const decimal StartSignalProcessing = 1.0m; //sec
        public const decimal SignalProcessing = 2.5m; //sec

        public const decimal DiffRepeatSignal = 0.5m;
        public const int SecSkip = 0;
        public const decimal Commission = 0.1m * 2;

        public const bool PriceEnable = true;
        public const bool FundingEnable = true;

        public const bool SMAEnable = false;
        public const int SMALen = 100;
        public const int IntervalCandle = 15;
        public const decimal limit_low = -2;
        public const decimal limit_up = 50;

        public const int IntervalPing = 20;
        public const float TimerWaitPong = 10.0f;

        public const int CoinsMax = 0;
        public static List<string> Coins = [];
        public static List<string> IgnoreCoins = ["BTC", "ETH", "USDC", "DAI", "XRP", "SOL", "DOGE", "FDUSD", "PEPE", "SUI", "BNB", "TRX", "DOGE", "SHIB", "LTC"];
        //"BTC", "DAI", "ETH","FDUSD","SOL","XRP","DOGE","BNB", "PEPE","SUI","ADA","TRX","LTC","AVAX","DOT,NEAR","ARB","UNI","XLM","FLOKI","ATOM","TON"
        //"BTC", "ETH", "USDC", "DAI", "XRP", "SOL", "DOGE", "FDUSD", "PEPE", "SUI", "BNB", "TRX", "DOGE", "SHIB", "LTC"
    }
}

/*
bybit: https://bybit-exchange.github.io/docs/v5/ws/connect
kucoin: https://www.kucoin.com/docs/websocket/spot-trading/public-channels/level2-market-data
mexc-spot: https://mexcdevelop.github.io/apidocs/spot_v3_en/#websocket-market-streams
mexc-future: https://mexcdevelop.github.io/apidocs/contract_v1_en/#websocket-api
gateio-spot: https://www.gate.io/docs/developers/apiv4/ws/en/#spot-websocket-v4
gateio-futures: https://www.gate.io/docs/developers/futures/ws/en/#order-book-api
htx: https://huobiapi.github.io/docs/spot/v1/en/?utm_source=chatgpt.com#subscribe-order-updates
htxF: https://huobiapi.github.io/docs/dm/v1/en/#subscribe-market-depth-data
//https://www.htx.com/en-us/opend/newApiPages/?id=8cb7c385-77b5-11ed-9966-0242ac110003
*/