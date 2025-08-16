using Arbitrage.Core.Base.Enums;
using Arbitrage.Other.Telegram;
using Arbitrage.Other;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Intrinsics.X86;
using System.Text;
using System.Threading.Tasks;
using Arbitrage.Core.Other;

namespace Arbitrage.Service.Base
{
    public class PreSignal
    {
        public string Id => "";
        public string Name;
        public AssetTypeEnum AssetTypeLong;
        public AssetTypeEnum AssetTypeShort;
        public ExchangeEnum ExchangeLong;
        public ExchangeEnum ExchangeShort;
        public Direct Direct;
        public decimal Target;
        public string Comment;

        public decimal? Result = null;

        public string ToMsg()
        {
            string msg = $"{Name}\n";
            try
            {
                msg += $"[{StringHelper.ExchangeStr(ExchangeLong, AssetTypeLong)}]({Network.GetUrl(Name, ExchangeLong, AssetTypeLong)}) 📗\n";
                msg += $"[{StringHelper.ExchangeStr(ExchangeShort, AssetTypeShort)}]({Network.GetUrl(Name, ExchangeShort, AssetTypeShort)}) 📕\n";
                msg += $"Цель: {StringHelper.DirectStr(Direct)} {Target}%\n";
                msg += $"Cейчас: {(Result != null ? $"{Result:0.00}" : "-")}%\n";
                msg += $"📈 [График]({Network.GetUrlArbService(AssetTypeLong, ExchangeLong, ExchangeShort, Name)})";
            }
            catch (Exception e)
            {
                msg += "не найден";
            }
            return msg;
        }
    }
}
