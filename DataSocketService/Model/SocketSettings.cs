using Arbitrage.ExchangeDomain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataSocketService.Model
{
    public class SocketSettings
    {
        public int WsCap;
        public int WsCapMax = 1;
        public bool CheckConnectByPing;
        public float TimerWaitPong;
        public int IntervalPing;


        public SocketSettings(int wsCap = int.MaxValue, int maxSub = 0, bool checkConnectByPing = true, float timerWaitPong = 0, int intervalPing = 0)
        {
            WsCap = wsCap;
            CheckConnectByPing = checkConnectByPing;

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
}
