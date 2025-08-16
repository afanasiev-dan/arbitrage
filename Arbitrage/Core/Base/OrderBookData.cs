using ArbitrageSignalBot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arbitrage.Service.Base
{
    public class OrderBookData
    {
        public List<(decimal price, decimal volume)> Asks { get; set; } = new();
        public List<(decimal price, decimal volume)> Bids { get; set; } = new();
        public DateTime dt;

        public decimal Ask => Asks.First().price;
        public decimal Bid => Bids.First().price;
        public decimal Spread => (Ask / Bid - 1) * 100;

        public double sec => (DateTime.Now - dt).TotalSeconds;
        public double mlSec => (DateTime.Now - dt).TotalMilliseconds;

        public bool isSuccess => !lag && isHaveData;
        public bool isHaveData => Bids.Count != 0 && Asks.Count != 0;
        bool lag => mlSec > 2500;

        public bool ChangeDt(DateTime dt, string ticker)
        {
            this.dt = dt;

            bool isStart = (DateTime.Now - Program.TimeStart).TotalSeconds > 10 && Asks.Count > 0 && Bids.Count > 0;
            if (mlSec > 10000 && isStart)
            {
                //Console.ForegroundColor = ConsoleColor.Red;
                //Console.WriteLine($"{ticker} {mlSec / 1000.0f:0.00} {dt:HH:mm:ss} {DateTime.Now:HH:mm:ss}");
                //Console.ForegroundColor = ConsoleColor.White;
                return false;
            }

            //var base_coin = ticker.Split("#")[1].Replace("]", "");
            //if (base_coin.ToLower() == "btc")
            //    Console.WriteLine($"{ticker} {ask}");
            return true;
        }
    }

    public class DescComparer : IComparer<decimal>
    {
        public int Compare(decimal x, decimal y) => y.CompareTo(x);
    }
}
