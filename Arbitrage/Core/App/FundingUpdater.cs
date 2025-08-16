using Arbitrage.Core.Base.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arbitrage.Core.App
{
    internal class FundingUpdater
    {
        private readonly ExchangeManager _exchangeManager;
        public bool Init;
        private TaskCompletionSource<bool> _initCompletionSource = new();

        public FundingUpdater(ExchangeManager exchangeManager)
        {
            _exchangeManager = exchangeManager;
        }

        public void Start()
        {
            Task.Run(async () =>
            {
                while (true)
                {
                    await UpdateFunding();
                    await Task.Delay((int)(LaunchConfig.FundingUpdate * 60 * 1000));
                }
            });
        }

        private async Task UpdateFunding()
        {
            foreach (var exchange in _exchangeManager.Exchanges)
            {
                if (exchange.Settings.AssetType == AssetTypeEnum.Futures)
                    await exchange.UpdateFunding();
            }
            Init = true;
            _initCompletionSource.TrySetResult(true);
        }

        public Task WaitForInitializationAsync()
        {
            if (Init)
                return Task.CompletedTask;

            return _initCompletionSource.Task;
        }
    }
}
