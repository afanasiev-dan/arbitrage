using Arbitrage.Core.Base.Enums;
using Arbitrage.Service.Base;
using ArbitrageSignalBot;

namespace Arbitrage.Other.Telegram
{
    public class TelegramBot
    {
        private readonly SignalManager signalManager;
        private readonly TelegramAPI _telegramApi;
        Dictionary<State, string> words = new()
        {
            { State.MainMenu, "Выберите действие:" },
            { State.PrintSignals, "Список сигналов:" },

            { State.AddingSignal, "Добавление нового сигнала..." },
            { State.EnteringCoin, "Название монеты:" },
            { State.EnteringLongExchange, "Биржа для лонга:" },
            { State.EnteringShortExchange, "Биржа для шорта:" },
            { State.EnteringDirectSignal, "Направление сделки:" },
            { State.EnteringResult, "Введите результат (%):" },
            { State.EnteringComment, "Комментарий к сигналу:" },

            { State.Settings, "Настройки бота:" },
            { State.InputInterval, "Введите интервал между одинаковыми сигналами (в минутах):" },
            { State.InputProfit, "Введите целевую прибыль по сигналу (%):" },
            //{ State.InputSlippage, "Введите макс.проскальзывание:" },
            { State.InputVolume, "Введите размер позиции (в USDT):" },
        };
        Dictionary<string, TelegramSignal> activate_signals = new();

        public TelegramBot(SignalManager signalManager)
        {
            this.signalManager = signalManager;
            _telegramApi = new TelegramAPI();

            SubscribeToEvents();
        }

        private void SubscribeToEvents()
        {
            _telegramApi.OnMessageReceived += HandleMessageReceived;
            _telegramApi.OnSignalRemoved += HandleSignalRemoved;
        }
        public async Task StopAsync()
        {
            _telegramApi.OnMessageReceived -= HandleMessageReceived;
            _telegramApi.OnSignalRemoved -= HandleSignalRemoved;
        }

        private async Task HandleMessageReceived(long chatId, string message)
        {
            try
            {
                //Console.WriteLine($"Received: {message} (State: {_currentState})");
                switch (message)
                {
                    case "/start":
                        signalManager.CreateSettings(chatId);
                        signalManager.settings[chatId]._currentState = State.MainMenu;
                        await SendMessage(chatId);
                        return;
                    case "Добавить":
                        signalManager.settings[chatId]._currentState = State.EnteringCoin;
                        signalManager.settings[chatId]._currentSignal = new PreSignal();
                        await SendMessage(chatId);
                        return;
                    case "Список":
                        await DisplaySignalList(chatId);
                        return;
                    case "Настройки":
                        signalManager.settings[chatId]._currentState = State.Settings;
                        await SendMessage(chatId);
                        return;
                    case "Назад":
                        ResetState(chatId);
                        await SendMessage(chatId, "в меню");
                        return;
                    default:
                        if (message.Contains("/print"))
                        {
                            string baseCoin = message.Split("/print")[1];
                            string m = Program.GetInfoCoin(baseCoin.Trim().ToLower());
                            await SendMessage(chatId, m, stateSignal: StateSignal.Auto);
                            string m2 = Program.GetInfoPair(baseCoin.Trim().ToLower());
                            await SendMessage(chatId, m2, stateSignal: StateSignal.Auto);
                        }
                        else
                        if (message.Contains("Интервал между сигналами"))
                        {
                            signalManager.settings[chatId]._currentState = State.InputInterval;
                            await SendMessage(chatId);
                            return;
                        }
                        else if (message.Contains("% прибыли"))
                        {
                            signalManager.settings[chatId]._currentState = State.InputProfit;
                            await SendMessage(chatId);
                            return;
                        }
                        //else if (message.Contains("% проскальзывания"))
                        //{
                        //    signalManager.settings[chatId]._currentState = State.InputSlippage;
                        //    await SendMessage(chatId);
                        //    return;
                        //}
                        else if (message.Contains("объем"))
                        {
                            signalManager.settings[chatId]._currentState = State.InputVolume;
                            await SendMessage(chatId);
                            return;
                        }
                        else if (message.Contains("Работает"))
                        {
                            bool new_state = !signalManager.settings[chatId].isWork;
                            signalManager.ChangeStateBot(chatId, new_state);
                            await SendMessage(chatId, new_state ? "Бот включен" : "Бот выключен");
                            return;
                        }
                        break;
                }
                switch (signalManager.settings[chatId]._currentState)
                {
                    case State.InputInterval:
                    case State.InputProfit:
                    case State.InputVolume:
                        //case State.InputSlippage:
                        await HandleSettings(chatId, message);
                        break;
                    case State.EnteringCoin:
                    case State.EnteringLongExchange:
                    case State.EnteringShortExchange:
                    case State.EnteringDirectSignal:
                    case State.EnteringResult:
                    case State.EnteringComment:
                        await HandleSignalCreation(chatId, message);
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error handling message: {ex.Message}");
                await _telegramApi.SendMessageAsync(chatId, "Произошла ошибка. Попробуйте еще раз.");
                ResetState(chatId);
            }
        }

        private async Task HandleSignalCreation(long chatId, string message)
        {
            switch (signalManager.settings[chatId]._currentState)
            {
                case State.EnteringCoin:
                    signalManager.settings[chatId]._currentSignal.Name = message;
                    signalManager.settings[chatId]._currentState = State.EnteringLongExchange;
                    await SendMessage(chatId);
                    break;
                case State.EnteringLongExchange:
                    if (message.Contains("#F"))
                    {
                        message = message.Split('#')[0];
                        signalManager.settings[chatId]._currentSignal.AssetTypeLong = AssetTypeEnum.Futures;
                    }
                    if (Enum.TryParse(message, out ExchangeEnum longExchange))
                    {
                        signalManager.settings[chatId]._currentSignal.ExchangeLong = longExchange;
                        signalManager.settings[chatId]._currentState = State.EnteringShortExchange;
                        await SendMessage(chatId);
                    }
                    else
                    {
                        ResetState(chatId);
                        await _telegramApi.SendMessageAsync(chatId, "Некорректная биржа. Возврат в меню", signalManager.settings[chatId]._currentState);
                    }
                    break;
                case State.EnteringShortExchange:
                    if (message.Contains("#F"))
                    {
                        message = message.Split('#')[0];
                        signalManager.settings[chatId]._currentSignal.AssetTypeShort = AssetTypeEnum.Futures;
                    }
                    if (Enum.TryParse(message, out ExchangeEnum shortExchange))
                    {
                        signalManager.settings[chatId]._currentSignal.ExchangeShort = shortExchange;
                        signalManager.settings[chatId]._currentState = State.EnteringDirectSignal;
                        await SendMessage(chatId);
                        //await _telegramApi.SendMessageAsync(chatId, "Введите результат:", signalManager.settings[chatId]._currentState);
                    }
                    else
                    {
                        await _telegramApi.SendMessageAsync(chatId, "Некорректная биржа. Попробуйте еще раз.", signalManager.settings[chatId]._currentState);
                    }
                    break;
                case State.EnteringDirectSignal:
                    signalManager.settings[chatId]._currentSignal.Direct = message.Contains("Вход") ? Direct.Enter : Direct.Exit;
                    signalManager.settings[chatId]._currentState = State.EnteringResult;
                    await SendMessage(chatId);
                    break;
                case State.EnteringResult:

                    message = message.Replace(".", ",");
                    if (decimal.TryParse(message, out decimal result))
                    {
                        signalManager.settings[chatId]._currentSignal.Target = result;
                        signalManager.settings[chatId]._currentState = State.EnteringComment;
                        await SendMessage(chatId);
                    }
                    else
                    {
                        await _telegramApi.SendMessageAsync(chatId, "Некорректный результат. Введите число.", signalManager.settings[chatId]._currentState);

                    }
                    break;
                case State.EnteringComment:
                    signalManager.settings[chatId]._currentSignal.Comment = message;
                    signalManager.AddSignal(chatId, signalManager.settings[chatId]._currentSignal);
                    signalManager.settings[chatId]._currentState = State.MainMenu;
                    await _telegramApi.SendMessageAsync(chatId, "Сигнал успешно добавлен!", signalManager.settings[chatId]._currentState);
                    break;
            }
        }
        private async Task HandleSettings(long chatId, string message)
        {
            switch (signalManager.settings[chatId]._currentState)
            {
                case State.InputProfit:
                    signalManager.settings[chatId]._currentState = State.Settings;
                    signalManager.ChangeProfit(chatId, decimal.Parse(message.Replace('.', ',')));
                    await SendMessage(chatId, $"Установлен уровень прибыли по сигналу на {message}%");
                    break;
                case State.InputVolume:
                    signalManager.settings[chatId]._currentState = State.Settings;
                    signalManager.ChangeVolume(chatId, decimal.Parse(message.Replace('.', ',')));
                    await SendMessage(chatId, $"Установлен размер позиции на {message} USDT");
                    break;
                case State.InputInterval:
                    signalManager.settings[chatId]._currentState = State.Settings;
                    signalManager.ChangeInterval(chatId, int.Parse(message));
                    await SendMessage(chatId, $"Установлен интервал между одинаковыми сигналами на {message} мин");
                    break;
                    //case State.InputSlippage:
                    //    signalManager.settings[chatId]._currentState = State.Settings;
                    //    signalManager.ChangeSlippage(chatId, decimal.Parse(message.Replace('.', ',')));
                    //    await SendMessage(chatId, $"Установлено проскальзывания {message} %");
                    //    break;

            }
        }
        public async Task SendMessage(long chatId, string message = "", StateSignal stateSignal = StateSignal.None, int? messageId = null)
        {
            if (string.IsNullOrEmpty(message))
                message = words[signalManager.settings[chatId]._currentState];
            await _telegramApi.SendMessageAsync(chatId, message, signalManager.settings[chatId]._currentState, settings: signalManager.GetSettings(chatId), stateSignal: stateSignal, messageId: messageId);
        }

        private async Task DisplaySignalList(long chatId)
        {
            var signals = signalManager.GetSignals(chatId);
            signalManager.settings[chatId]._currentState = State.PrintSignals;

            if (signals == null || signals.Count == 0)
                await _telegramApi.SendMessageAsync(chatId, "Список сигналов пуст.");
            else
                signals.ForEach(async x => await PrintMadeSignal(chatId, x));

            ResetState(chatId);
        }
        private async Task HandleSignalRemoved(long chatId, string id)
        {
            bool flag = signalManager.RemoveSignal(chatId, id);
            await _telegramApi.SendMessageAsync(chatId, $"Сигнал {(flag ? "" : "не")} удален.");
            //await DisplaySignalList(chatId);
        }
        private void ResetState(long chatId)
        {
            signalManager.settings[chatId]._currentState = State.MainMenu;
            signalManager.settings[chatId]._currentSignal = null;
        }

        public async void ActivateSignal(long chatId, Settings settings, Signal signal, StateSignal stateSignal = StateSignal.Made)
        {
            GroupSignal group = new([signal]);
            ActivateSignal(chatId, settings, group, stateSignal);
        }
        public async void ActivateSignal(long chatId, Settings settings, GroupSignal group, StateSignal stateSignal = StateSignal.Made, int? messageId = null)
        {
            var signal = group.Signal;
            if (!activate_signals.ContainsKey(signal.Id))
            {
                activate_signals.Add(signal.Id, new(DateTime.Now, group.Signal.Result));
            }
            else
            {
                var total_minutes = (DateTime.Now - activate_signals[signal.Id].time).TotalMinutes;
                var profit_up = group.Signal.Result - activate_signals[signal.Id].result > LaunchConfig.DiffRepeatSignal;
                if (settings != null && !(total_minutes > settings.interval || (profit_up && !activate_signals[signal.Id].mute)))
                {
                    if (messageId == null)
                        return;
                }
            }

            await PrintSignal(chatId, group, stateSignal, messageId);
            activate_signals[signal.Id].time = DateTime.Now;
            activate_signals[signal.Id].result = group.Signal.Result;
            activate_signals[signal.Id].mute = false;
        }
        public void ClearSignal(string id)
        {
            if (activate_signals.ContainsKey(id))
                activate_signals.Remove(id);
        }
        async Task PrintSignal(long chatId, GroupSignal group, StateSignal state = StateSignal.None, int? messageId = null)
        {
            var signal = group.Signal;
            string message = string.Empty;
            switch (state)
            {
                case StateSignal.Made:
                    message = "📢 Сигнал сработал 📢\n\n";
                    break;
                case StateSignal.Auto:
                    message = string.Empty;
                    break;
            }
            message += signal.ToMsg(state);

            Dictionary<string, string> buttons = new();

            if (signalManager.settings[chatId]._currentState == State.PrintSignals)
                buttons.Add("Удалить", $"RS {signal.Id}");
            else if(state == StateSignal.Auto || state == StateSignal.MadePrint)
                buttons = group.GetButtons(chatId);

            await _telegramApi.SendMessageAsync(chatId, message, 
                signalManager.settings[chatId]._currentState,
                arguments: buttons, 
                stateSignal: state, 
                messageId: messageId);
        }
        async Task PrintMadeSignal(long chatId, PreSignal signal)
        {
            string message = signal.ToMsg();
            Dictionary<string, string> buttons = new()
            {
                { "Удалить", $"RS {signal.Id}" }
            };

            await _telegramApi.SendMessageAsync(chatId, message, 
                signalManager.settings[chatId]._currentState, 
                arguments: buttons, 
                stateSignal: StateSignal.Made);
        }

        public DateTime Mute(string key)
        {
            if (activate_signals.ContainsKey(key))
                activate_signals[key].mute = true;

            var new_time = activate_signals[key].time.AddHours(2);
            activate_signals[key].time = new_time;
            return new_time;
        }
    }

    public class TelegramSignal
    {
        public DateTime time;
        public decimal result;
        public bool mute;

        public TelegramSignal(DateTime time, decimal result)
        {
            this.time = time;
            this.result = result;
        }
    }
    public enum StateSignal
    {
        None, Made, MadePrint, Auto
    }
    public enum State
    {
        MainMenu,
        AddingSignal,
        EnteringCoin,
        EnteringLongExchange,
        EnteringShortExchange,
        EnteringDirectSignal,
        EnteringResult,
        EnteringComment,
        PrintSignals,

        Settings,
        InputInterval,
        InputProfit,
        //InputSlippage,
        InputVolume
    }
}
