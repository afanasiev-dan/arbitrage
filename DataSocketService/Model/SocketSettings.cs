using DataSocketService.Exchanges.Base;

namespace DataSocketService.Model
{
    public class SocketSettings
    {
        public int WsCap;
        public int WsCapMax { get; } = 1;
        public bool CheckConnectByPing;
        public float TimerWaitPong;
        public int IntervalPing;
        public Func<SocketBase> SocketCreator { get; }

        public SocketSettings(
            Func<SocketBase> socketCreator,
            int wsCap = int.MaxValue,
            int maxSub = 0,
            bool checkConnectByPing = true,
            float timerWaitPong = 0,
            int intervalPing = 0)
        {
            SocketCreator = socketCreator;
            WsCap = wsCap;
            CheckConnectByPing = checkConnectByPing;

            TimerWaitPong = timerWaitPong == 0 ? LaunchConfig.TimerWaitPong : timerWaitPong;
            IntervalPing = intervalPing == 0 ? LaunchConfig.IntervalPing : intervalPing;

            TimerWaitPong *= LaunchConfig.xWait;
        }
    }
}
