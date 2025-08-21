using Arbitrage.ExchangeDomain;
using Arbitrage.ExchangeDomain.Enums;
using DataSocketService.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Arbitrage.ExchangeDomain;

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
            {$"{Arbitrage.ExchangeDomain.Exchanges.Gate}-{MarketType.Spot}", new()},
            {$"{Arbitrage.ExchangeDomain.Exchanges.Gate}-{MarketType.Futures}", new()},

            {$"{Arbitrage.ExchangeDomain.Exchanges.ByBit}-{MarketType.Spot}", new()},
            {$"{Arbitrage.ExchangeDomain.Exchanges.ByBit}-{MarketType.Futures}", new()},

            {$"{Arbitrage.ExchangeDomain.Exchanges.KuCoin}-{MarketType.Spot}", new()},
            {$"{Arbitrage.ExchangeDomain.Exchanges.KuCoin}-{MarketType.Futures}", new()},

            {$"{Arbitrage.ExchangeDomain.Exchanges.Mexc}-{MarketType.Spot}", new()},
            {$"{Arbitrage.ExchangeDomain.Exchanges.Mexc}-{MarketType.Futures}", new()},

            {$"{Arbitrage.ExchangeDomain.Exchanges.Htx}-{MarketType.Spot}", new() { CheckConnectByPing = false, TimerWaitPong = 20 } },
            {$"{Arbitrage.ExchangeDomain.Exchanges.Htx}-{MarketType.Futures}", new() {CheckConnectByPing = false, TimerWaitPong = 20} },

            {$"{Arbitrage.ExchangeDomain.Exchanges.LBank}-{MarketType.Spot}", new() {WsCap = 50} },
        };
    }
}
