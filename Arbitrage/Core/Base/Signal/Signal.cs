using Arbitrage.Core.App;
using Arbitrage.Core.Base.Enums;
using Arbitrage.Core.Calclucation;
using Arbitrage.Other;
using Arbitrage.Other.Telegram;

namespace Arbitrage.Service.Base
{
    public class Signal : ResultPair
    {
        public string Id => $"{Name} {isAuto} {(string.IsNullOrEmpty(Comment) ? "-" : Comment)} {Target} {TargetVolume}";
        public string Name => LongPos.Info.Coin.BaseCoin;

        public string Comment;
        public decimal TargetVolume;
        public decimal Target;
        public Direct Direct;
        public bool isAuto;

        public static bool Compare(PreSignal p1, ResultPair p2)
        {
            bool s1 = p1.ExchangeLong == p2.LongPos.Exchange.Settings.Name;
            bool s2 = p1.ExchangeShort == p2.ShortPos.Exchange.Settings.Name;
            bool s3 = p1.AssetTypeLong == p2.LongPos.Exchange.Settings.AssetType;
            bool s4 = p1.AssetTypeShort == p2.ShortPos.Exchange.Settings.AssetType;
            bool s5 = p1.Name.ToLower() == p2.LongPos.Info.Coin.BaseCoin.ToLower();
            return s1 && s2 && s3 && s4 && s5;
        }

        public void Attach(ResultPair pair)
        {
            LongPos = pair.LongPos;
            ShortPos = pair.ShortPos;
            LongPrice = pair.LongPrice;
            ShortPrice = pair.ShortPrice;
            Spread = pair.Spread;
            SMA = pair.SMA;
            HasError = pair.HasError;
            SuccessDT = pair.SuccessDT;
        }

        public Signal (ResultPair pair) 
        {
            Attach(pair);
        }

        public Signal (PreSignal preSignal)
        {
            Target = preSignal.Target;
            Comment = preSignal.Comment;
            Direct = preSignal.Direct;

            CoinInfo coinInfo = new()
            {
                BaseCoin = preSignal.Name
            };
            ExchangeAssetInfo ExchLongInfo = new(preSignal.ExchangeLong, preSignal.AssetTypeLong);
            ExchangeAssetInfo ExchShortInfo = new(preSignal.ExchangeShort, preSignal.AssetTypeShort);
            LongPos = new()
            {
                Info = new(coinInfo, preSignal.AssetTypeLong),
                Exchange = ExchangeManager.CreateExchange(ExchLongInfo),
            };  
            ShortPos = new()
            {
                Info = new(coinInfo, preSignal.AssetTypeShort),
                Exchange = ExchangeManager.CreateExchange(ExchShortInfo),
            };
        }
        public string ToMsg(StateSignal stateSignal)
        {
            string msg = $"{Name}\n";
            try
            {
                var longSpread = LongPos.Book.Spread;
                var shortSpread = ShortPos.Book.Spread;

                msg += $"[{LongPos.Exchange.Settings.Print()}]({Network.GetUrl(Name, LongPos.Exchange.Settings.Name, LongPos.Exchange.Settings.AssetType)}) 📗\n";
                msg += $"Cтакан: {longSpread:0.00}% \n"; //{dt_update_long:mm:ss}
                if (LongPos.Info.AssetType == AssetTypeEnum.Futures)
                    msg += LongPos.Funding.Print() + "\n";
                msg += "\n";
                msg += $"[{ShortPos.Exchange.Settings.Print()}]({Network.GetUrl(Name, ShortPos.Exchange.Settings.Name, ShortPos.Exchange.Settings.AssetType)}) 📕 \n";
                msg += $"Стакан: {ShortPos.Book.Spread:0.00}% \n";
                if (ShortPos.Info.AssetType == AssetTypeEnum.Futures)
                    msg += ShortPos.Funding.Print() + "\n";
                msg += "\n";

                switch (stateSignal)
                {
                    case StateSignal.MadePrint:
                    case StateSignal.Made:
                        msg += $"Цель: {Target}% Cейчас: {(Result != null ? $"{Result:0.00}" : "-")}%\n";
                        msg += $"Курсовой: {(Spread != null ? $"{Spread:0.00}" : "-")}% \n";
                        msg += $"SMA: {SMA:0.00}% \n";
                        break;
                    case StateSignal.Auto:
                        msg += $"Время жизни: {LifeTimeSec/60} мин \n";
                        msg += $"Курсовой: {Spread:0.00}% " + "{" + $"{LongPrice} {ShortPrice}" + "}\n";
                        msg += $"SMA: {SMA:0.00}% \n";
                        msg += $"Фандинг: {ResultFund:0.00}% \n";
                        msg += $"Комиссия: -{LaunchConfig.Commission:0.00}% \n";
                        msg += $"Стакан: -{longSpread + shortSpread:0.00}% \n";
                        msg += $"Прибыль: {Result:0.00}%\n";
                        break;
                }
                if (!(string.IsNullOrEmpty(Comment) || Comment == "-"))
                    msg += $"{Comment}\n";

                msg += "\n";
                msg += $"📈 [График]({Network.GetUrlArbService(LongPos.Exchange.Settings.AssetType, LongPos.Exchange.Settings.Name, ShortPos.Exchange.Settings.Name, Name)})";
            }
            catch (Exception e)
            {
                msg += "не найден";
            }
            return msg;
        }
        public string ToMiniMsg()
        {
            string l = $"{LongPos.Exchange.Settings.Print()}";// ExchangeLong}{MiniAssetType(AssetTypeLong)}";
            string s = $"{ShortPos.Exchange.Settings.Print()}";
            return $"{l} - {s} 🎯 {Result:0.00}%";
        }
    }

    public enum Direct
    {
        Enter, Exit
    }
}
