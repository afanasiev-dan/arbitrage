using Arbitrage.ExchangeDomain;
using Arbitrage.ExchangeDomain.Enums;
using DataSocketService.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            {$"{Exchanges.Gate}-{MarketType.Spot}", new()},
            {$"{Exchanges.Gate}-{MarketType.Futures}", new()},

            {$"{Exchanges.ByBit}-{MarketType.Spot}", new()},
            {$"{Exchanges.ByBit}-{MarketType.Futures}", new()},

            {$"{Exchanges.KuCoin}-{MarketType.Spot}", new()},
            {$"{Exchanges.KuCoin}-{MarketType.Futures}", new()},

            {$"{Exchanges.Mexc}-{MarketType.Spot}", new()},
            {$"{Exchanges.Mexc}-{MarketType.Futures}", new()},

            {$"{Exchanges.Htx}-{MarketType.Spot}", new() { CheckConnectByPing = false, TimerWaitPong = 20 } },
            {$"{Exchanges.Htx}-{MarketType.Futures}", new() {CheckConnectByPing = false, TimerWaitPong = 20} },

            {$"{Exchanges.LBank}-{MarketType.Spot}", new() {WsCap = 50} },
        };
    }
}
