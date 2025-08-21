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

        public void ChangeDt(DateTime time)
        {
            Time = time;
        }
    }
}
