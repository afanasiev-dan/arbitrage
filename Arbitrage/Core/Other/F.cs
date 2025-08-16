using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace Arbitrage.Other
{
    public class F
    {
        public static decimal ToDec(string value)
        {
            value = value.Replace(" ", "").Replace(",", ".");
            return decimal.Parse(value,
            NumberStyles.Float | NumberStyles.AllowExponent,
            CultureInfo.InvariantCulture);
        }
        public static bool CompareDateTime(DateTime dt1, DateTime dt2)
        {
            return dt1.Year == dt2.Year && dt1.Month == dt2.Month && dt1.Day == dt2.Day && dt1.Hour == dt2.Hour && dt1.Minute == dt2.Minute;
        }
        private static Stopwatch _stopwatch = new();

        public static void dateFromr()
        {
            _stopwatch.Restart();
        }
        public static void StopTimer()
        {
            _stopwatch.Stop();
            Console.WriteLine($"{_stopwatch.ElapsedMilliseconds} мс");
        }
    }

    public class MyTimer
    {
        private Stopwatch stopwatch;

        public double ResultD => stopwatch.Elapsed.TotalMilliseconds;
        public string Result => $"{ResultD:0.00000} ms";

        public MyTimer()
        {
            stopwatch = Stopwatch.StartNew();
        }

        public void Pause()
        {
            if (stopwatch.IsRunning)
                stopwatch.Stop();
        }

        public void Resume()
        {
            if (!stopwatch.IsRunning)
                stopwatch.Start();
        }

        public void Reset()
        {
            stopwatch.Reset();
            stopwatch.Start();
        }

        public TimeSpan Elapsed => stopwatch.Elapsed;
    }

    public static class ListExtensions
    {
        private static Random rng = new Random();

        public static void Shuffle<T>(this IList<T> list)
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }
    }

}
