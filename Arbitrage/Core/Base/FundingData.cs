using Arbitrage.Core.Base.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arbitrage.Core.Base
{
    public class FundingData
    {
        public decimal? Value = null;
        public DateTime? TimePay = null;
        public FundingType Type;
        public int Interval;

        public bool IsSuccess(AssetTypeEnum assetType)
        {
            return Value != null && assetType == AssetTypeEnum.Futures;
        }
        public string Print()
        {
            string m = string.Empty;
            m += $"Ставка: {Value:0.00}% \n";
            m += $"Тип: {Type} \n";
            m += $"Интервал: {Interval} \n";
            m += $"Время: {TimePay:HH:mm}";
            return m;
        }
    }

    //OKX / KuCoin / Huobi / Coinex - FIX
    //Binance / ByBit / BitGet / MexC / LBank / BitMart / XT - FLOAT
    public enum FundingType
    {
        Fix, Float
    }
}
