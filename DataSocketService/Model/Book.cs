using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataSocketService.Model
{
    public class Book
    {
        public List<(decimal price, decimal volume)> Asks = new();
        public List<(decimal price, decimal volume)> Bids = new();
        public DateTime Time;

        public decimal Ask => Asks.Count > 0 ? Asks.First().price : -1;
        public decimal Bid => Bids.Count > 0 ? Bids.First().price : -1;

        public void ChangeDt(DateTime time)
        {
            Time = time;
        }
    }
}
