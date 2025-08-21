using Arbitrage.ExchangeDomain.Enums;
using Arbitrage.Service.Gateio;
using DataSocketService.Exchanges;
using DataSocketService.Exchanges.Base;
using DataSocketService.Model;

namespace DataSocketService
{
    public class LaunchConfig
    {
        public const int IntervalPing = 20;
        public const float TimerWaitPong = 10.0f;

        public const int wsCap = 30;
        public const float xWait = 2;

        public static Dictionary<string, SocketSettings> SocketSettings = new()
        {
            {$"{Arbitrage.ExchangeDomain.Exchanges.Gate}-{MarketType.Spot}",
                new(() => new GateioSocket())},
            {$"{Arbitrage.ExchangeDomain.Exchanges.Gate}-{MarketType.Futures}",
                new(() => new GateioSocket())},

            {$"{Arbitrage.ExchangeDomain.Exchanges.ByBit}-{MarketType.Spot}",
                new(() => new ByBitSocket())},
            {$"{Arbitrage.ExchangeDomain.Exchanges.ByBit}-{MarketType.Futures}",
                new(() => new ByBitSocket())},

            {$"{Arbitrage.ExchangeDomain.Exchanges.KuCoin}-{MarketType.Spot}", 
                new(() => new ByBitSocket())},
            {$"{Arbitrage.ExchangeDomain.Exchanges.KuCoin}-{MarketType.Futures}", 
                new(() => new ByBitSocket())},

            {$"{Arbitrage.ExchangeDomain.Exchanges.Mexc}-{MarketType.Spot}", 
                new(() => new ByBitSocket())},
            {$"{Arbitrage.ExchangeDomain.Exchanges.Mexc}-{MarketType.Futures}", 
                new(() => new ByBitSocket())},

            {$"{Arbitrage.ExchangeDomain.Exchanges.Htx}-{MarketType.Spot}", 
                new(() => new ByBitSocket(), checkConnectByPing:false, timerWaitPong:20) },
            {$"{Arbitrage.ExchangeDomain.Exchanges.Htx}-{MarketType.Futures}", 
                new(() => new ByBitSocket(), checkConnectByPing:false, timerWaitPong:20) },

            {$"{Arbitrage.ExchangeDomain.Exchanges.LBank}-{MarketType.Spot}", 
                new(() => new ByBitSocket(), wsCap:50)},
        };
    }
}
