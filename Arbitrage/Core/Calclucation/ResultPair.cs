using Arbitrage.Service.Base;

namespace Arbitrage.Core.Calclucation
{
    public class ResultPair
    {
        public Crypto LongPos;
        public Crypto ShortPos;

        public decimal LongPrice;
        public decimal ShortPrice;
        public decimal Spread;
        public decimal SMA;
        public decimal Result
        {
            get
            {
                var result = Spread + ResultFund - SMA * 0.75m;
                if(LongPos.Book.isHaveData && ShortPos.Book.isHaveData)
                    return result - LaunchConfig.Commission - (ShortPos.Book.Spread + LongPos.Book.Spread);
                else
                    return result - LaunchConfig.Commission;
            }
        }
        public decimal ResultFund
        {
            get
            {
                return MathCups.GetProfitFund(LongPos.Funding.TimePay, LongPos.Funding.Value, ShortPos.Funding.TimePay, ShortPos.Funding.Value);
            }
        }
        
        public bool HasError;
        public DateTime? SuccessDT;
        public int LifeTimeSec
            => SuccessDT == null? 0 : (int)(DateTime.Now - (DateTime)SuccessDT).TotalSeconds;
        
        public void CalculateSpred(decimal volume)
        {
            try
            {
                var asksCopy = new List<(decimal price, decimal volume)>(LongPos.Book.Asks);
                var bidsCopy = new List<(decimal price, decimal volume)>(ShortPos.Book.Bids);

                var longResult = CupsCalculation.CalculateAverageEntryPrice(asksCopy, volume, true);
                var shortResult = CupsCalculation.CalculateAverageEntryPrice(bidsCopy, volume);
                LongPrice = longResult.price;
                ShortPrice = shortResult.price;

                //var LongPrice2 = CupsCalculation.CalculateAverageEntryPrice(asksCopy, 1);
                // ShortPrice2 = CupsCalculation.CalculateAverageEntryPrice(bidsCopy, 1);
                if(!longResult.success)
                {
                    Spread = -100000 - longResult.price;
                }
                else if (!shortResult.success)
                {
                    Spread = -200000 - shortResult.price;
                }
                else
                    Spread = MathCups.ExchangeRateSpread(LongPrice, ShortPrice);
                //var Spread2 = MathCups.ExchangeRateSpread(LongPrice2, ShortPrice2);
            }
            catch (Exception ex)
            {
            
            }
        }
        public void CalculateSMA()
        {
            var candles_l = LongPos.Prices;
            var candles_s = ShortPos.Prices;
            SMA = MathCups.GetSpreadSMA(candles_l, candles_s, 100);
        }
        public ResultPair Clone()
            => (ResultPair) MemberwiseClone();

        public string Print()
        {
            string str = $"{LongPos.Exchange.Settings.Print()} {ShortPos.Exchange.Settings.Print()} S:{Spread:0.00}% R:{Result:0.00}";
            return str;
        }
    }
}
