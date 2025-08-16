using Arbitrage.Core.Calclucation;
using Arbitrage.Core.Other;
using Arbitrage.Other.Telegram;
using Arbitrage.Service.Base;

namespace Arbitrage.Core.App
{
    public class SignalProcessor
    {
        private readonly SignalManager _signalManager;
        public readonly TelegramBot telegramBot;
        public List<ResultPair> pairs;
        public Dictionary<long, List<Signal>> signals = new();
        public List<GroupSignal> GroupedSignals = new();

        public SignalProcessor(SignalManager signalManager, TelegramBot telegramBot, List<Crypto> cryptos)
        {
            _signalManager = signalManager;
            this.telegramBot = telegramBot;
            pairs = PairHelper.CreatePairs(cryptos);
        }

        public void Start()
        {
            Task.Run(async () =>
            {
                await Task.Delay((int)(LaunchConfig.StartSignalProcessing * 1000));
                while (true)
                {
                    await UpdatePairs();
                    await Task.Delay((int)(LaunchConfig.SignalProcessing * 1000));
                }
            });
        }

        public async Task ActivateSignal(long chatId, int messageId, string baseCoin)
        {
            var group = GroupedSignals.Find(x => x.Signal.Name == baseCoin);
            if (group != null)
            {
                var settings = _signalManager.settings[chatId];
                telegramBot.ActivateSignal(chatId, settings, group, StateSignal.Auto, messageId);
            }
            else
            {
                await telegramBot.SendMessage(chatId, $"{baseCoin} сигнал не актуален", StateSignal.Auto, messageId);
            }
        }

        private async Task UpdatePairs()
        {
            try
            {
                var tasks = _signalManager.settings
                    .Select(async account => new
                    {
                        ChatId = account.Key,
                        Signals = await ProcessAccount(account.Key, account.Value)
                    })
                    .ToList();

                var results = await Task.WhenAll(tasks);

                signals = results.ToDictionary(
                    result => result.ChatId,
                    result => result.Signals
                );
                var groupedSignals = signals
                    .SelectMany(pair => pair.Value.Select(signal => new { ChatId = pair.Key, Signal = signal }))
                    .GroupBy(item => (item.ChatId, item.Signal.Name));

                List<GroupSignal> groups = new();
                foreach (var group in groupedSignals)
                {
                    long chatId = group.Key.ChatId;
                    string coinName = group.Key.Name;
                    var signals = group.Select(g => g.Signal).OrderByDescending(s => s.Result).ToList();

                    var groupedSignal = new GroupSignal(signals);
                    groups.Add(groupedSignal);
                    var settings = _signalManager.settings[chatId];
                    telegramBot.ActivateSignal(chatId, settings, groupedSignal, StateSignal.Auto);
                }
                GroupedSignals = groups;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private async Task<List<Signal>> ProcessAccount(long chatId, Settings settings)
        {
            PairHelper.UpdatePairs(pairs, settings.volume);
            List<Signal> signals = new();
            foreach (var pair in pairs)
            {
                if (pair.HasError) continue;

                //auto
                if (settings.isWork)
                {
                    try
                    {
                        //Console.WriteLine(pair.LifeTimeSec);
                        if (pair.Result > settings.profit)
                        {
                            if (pair.SuccessDT == null)
                            {
                                pair.SuccessDT = DateTime.Now;
                                //Console.WriteLine($"таймер запущен {DateTime.Now:HH}");
                            }
                            if (pair.LifeTimeSec >= LaunchConfig.SecSkip)
                            {
                                Signal signal = new(pair);
                                signal.TargetVolume = settings.volume;
                                signal.isAuto = true;
                                signals.Add(signal);
                            }
                        }
                        else
                        {
                            pair.SuccessDT = null;
                            //Console.WriteLine($"обнулен-1 {pair.Result} {settings.profit}");
                        }
                    }
                    catch (Exception ex)
                    {
                        //Console.WriteLine($"[pair] {ex.Message}");
                    }
                }

                //made
                foreach (var madeSignal in settings.madeSignals)
                {
                    if (Signal.Compare(madeSignal, pair))
                    {
                        Signal signal = new(pair);
                        signal.Target = madeSignal.Target;
                        madeSignal.Result = pair.Result;
                        signal.isAuto = false;

                        if (pair.Result >= signal.Target && madeSignal.Direct == Direct.Enter)
                            telegramBot.ActivateSignal(chatId, settings, signal);
                        if (pair.Result <= signal.Target && madeSignal.Direct == Direct.Exit)
                            telegramBot.ActivateSignal(chatId, settings, signal);
                    }
                }
            }
            return signals;
        }
    }
}
