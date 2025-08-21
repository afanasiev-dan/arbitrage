using System;
using System.Globalization;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Timers;
using Arbitrage;
using Arbitrage.Core;
using Arbitrage.Core.Base;
using Arbitrage.Core.Base.Enums;
using Arbitrage.Other;
using Arbitrage.Service.Base;
using Newtonsoft.Json.Linq;
using Timer = System.Timers.Timer;

class KuCoinAPI : Exchange
{
    public KuCoinAPI(ExchangeAssetInfo settings) : base(settings)
    {
    }

    public override async Task Init(int size)
    {
        await base.Init(size);
    }

    public override SocketBook CreateSocketBook()
        => new KuCoinSocket();
}