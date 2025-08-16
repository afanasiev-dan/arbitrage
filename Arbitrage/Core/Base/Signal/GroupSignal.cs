using Arbitrage.Core.Calclucation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace Arbitrage.Service.Base
{
    public class GroupSignal
    {
        public List<Signal> signals;
        public Signal Signal => signals.First();

        public GroupSignal(List<Signal> signals)
        {
            this.signals = signals;
        }
        public Dictionary<string, string> GetButtons(long chatId)
        {
            var result = new Dictionary<string, string> { { "🔄", $"R {Signal.Id}" }, { "❌", "D" }, { "🔕", $"M {Signal.Id}" } };
            foreach (var x in signals.Skip(1))
                result.Add(x.ToMiniMsg(), $"P {x.Id}");
            return result;
        }
    }
}
