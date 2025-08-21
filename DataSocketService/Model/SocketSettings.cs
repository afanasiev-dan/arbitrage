namespace DataSocketService.Model
{
    public class SocketSettings
    {
        public int WsCap { get; }
        public int WsCapMax { get; } = 1;
        public bool CheckConnectByPing { get; }
        public float TimerWaitPong { get; private set; }
        public int IntervalPing { get; }

        public SocketSettings(
            int wsCap = int.MaxValue,
            int maxSub = 0,
            bool checkConnectByPing = true,
            float timerWaitPong = 0,
            int intervalPing = 0)
        {
            WsCap = wsCap;
            CheckConnectByPing = checkConnectByPing;

            TimerWaitPong = timerWaitPong == 0 ? LaunchConfig.TimerWaitPong : timerWaitPong;
            IntervalPing = intervalPing == 0 ? LaunchConfig.IntervalPing : intervalPing;

            TimerWaitPong *= LaunchConfig.xWait;
        }
    }
}
