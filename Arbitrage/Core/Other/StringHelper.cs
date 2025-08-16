using Arbitrage.Core.Base.Enums;
using Arbitrage.Service.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arbitrage.Core.Other
{
    internal class StringHelper
    {
        public static string ExchangeStr(ExchangeEnum Name, AssetTypeEnum AssetType)
            => $"{Name}{MiniAssetType(AssetType)}";
        static string MiniAssetType(AssetTypeEnum type)
            => type == AssetTypeEnum.Futures ? "#F" : string.Empty;

        public static string DirectStr(Direct direct)
        {
            switch(direct)
            {
                case Direct.Enter:
                    return ">";
                case Direct.Exit:
                    return "<";
                default:
                    return "?";
            }
        }
    }
}
