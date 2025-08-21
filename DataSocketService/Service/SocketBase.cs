using Arbitrage.ExchangeDomain;
using Arbitrage.ExchangeDomain.Enums;
using Arbitrage.Symbols.Presentation.Dto.CurrencyPair;
using DataSocketService.Model;
using DataSocketService.Other;
using Newtonsoft.Json;
using System;
using System.Buffers;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace DataSocketService.Service
{
    public abstract class SocketBase
    {
        protected ExchangeBase myParent;
        protected string Name => $"{myParent.Name}-{id}";
        protected SocketSettings Settings => myParent.Settings;

        protected string url => myParent.Info.Type == MarketType.Spot ? urlSpot : urlFutures;
        protected abstract string urlFutures { get; }
        protected abstract string urlSpot { get; }

        public ConcurrentDictionary<string, CurrencyPairBook> currencyPairDict = new();
        protected string id;

        protected ClientWebSocket _webSocket;
        private readonly CancellationTokenSource _cts = new CancellationTokenSource();

        private bool _isReconnecting = false;
        private readonly object _reconnectLock = new();
        private int _reconnectAttempts = 0;
        private const int BaseReconnectDelayMs = 1000;

        private Timer _pingTimer;
        protected System.Timers.Timer timerWaitPong;
        DateTime time_send_ping;
        DateTime time_send_pong;

        public virtual void Init(ExchangeBase myParent, string id)
        {
            this.myParent = myParent;
            this.id = id;

            _webSocket = new ClientWebSocket();

            var timerCheckPong = TimeSpan.FromSeconds(Settings.TimerWaitPong);
            timerWaitPong = new System.Timers.Timer(timerCheckPong);
            timerWaitPong.Elapsed += async (sender, e) => await ReConnect();
            timerWaitPong.AutoReset = false;
        }

        bool IsAddCoin() => Settings.WsCap > currencyPairDict.Count || Settings.WsCap == 0;

        public virtual bool AddToSocket(CurrencyPairResponceDto currencyPair)
        {
            if (!IsAddCoin()) return false;
            var ticker = currencyPair.Ticker;
            CurrencyPairBook currencyPairBook = new(currencyPair);
            currencyPairDict[ticker] = currencyPairBook;
            return true;
        }

        public virtual async Task ConnectAsync()
        {
            var chunked = currencyPairDict.Values
              .Select((crypto, index) => new { crypto, index })
              .GroupBy(x => x.index / Settings.WsCapMax)
              .Select(g => g.Select(x => x.crypto.Info).ToList())
              .ToList();

            foreach (var chunk in chunked)
            {
                string[] messages = chunk.Select(info => MessageSubscribe(info)).ToArray();
                var json = JsonConvert.SerializeObject(MessageSubscribeAll(messages));
                Console.WriteLine(json);
                await _webSocket.SendAsync(
                    Encoding.UTF8.GetBytes(json),
                    WebSocketMessageType.Text,
                    true,
                    CancellationToken.None
                );
            }
            StartPingTimer();
            _ = Task.Run(ListenAsync);

        }
        async Task ReConnect()
        {
            lock (_reconnectLock)
            {
                if (_isReconnecting) return;
                _isReconnecting = true;
            }

            try
            {
                //int sum = updateDict.Values.Sum(x => x.MessageChannel.Reader.Count);
                _reconnectAttempts++;
                timerWaitPong.Stop();

                int delayMs = (int)(BaseReconnectDelayMs * Math.Pow(2, _reconnectAttempts - 1));
                Console.WriteLine($"[{Name}] {_webSocket.State} {time_send_ping:mm:ss} {time_send_pong:mm:ss} {DateTime.Now:mm:ss} Попытка #{_reconnectAttempts} через {delayMs} мс...");
                await Task.Delay(delayMs);

                _webSocket = new ClientWebSocket();
                await ConnectAsync();

                //Console.WriteLine($"[{Settings.Print()}] Успешно переподключено.");
                _reconnectAttempts = 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[{Name} Reconnect] Ошибка: {ex.Message}");
                await ReConnect();
            }
            finally
            {
                lock (_reconnectLock)
                {
                    _isReconnecting = false;
                }
            }
        }

        private readonly ArrayPool<byte> _pool = ArrayPool<byte>.Shared;
        async IAsyncEnumerable<byte[]> ReadWebSocketMessagesAsync()
        {
            var buffer = _pool.Rent(8192);
            try
            {
                while (_webSocket.State == WebSocketState.Open && !_cts.IsCancellationRequested)
                {
                    int totalBytes = 0;
                    byte[] payload = null;

                    try
                    {
                        WebSocketReceiveResult result;
                        do
                        {
                            var segment = new ArraySegment<byte>(buffer, totalBytes, buffer.Length - totalBytes);
                            result = await _webSocket.ReceiveAsync(segment, _cts.Token);
                            totalBytes += result.Count;

                            if (totalBytes >= buffer.Length)
                            {
                                var newBuffer = _pool.Rent(buffer.Length * 2);
                                Buffer.BlockCopy(buffer, 0, newBuffer, 0, totalBytes);
                                _pool.Return(buffer);
                                buffer = newBuffer;
                            }
                        }
                        while (!result.EndOfMessage);

                        if (result.MessageType == WebSocketMessageType.Text || result.MessageType == WebSocketMessageType.Binary)
                        {
                            payload = DecodingF.Normalize(buffer.AsSpan(0, totalBytes).ToArray());
                        }
                        else if (result.MessageType == WebSocketMessageType.Close)
                        {
                            break;
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"[{Name} ReadWeb] {ex.Message}");
                        ReConnect();
                        break;
                    }

                    if (payload != null && payload.Length > 0)
                        yield return payload;
                }
            }
            finally
            {
                _pool.Return(buffer);
            }
        }
        async Task ListenAsync()
        {
            try
            {
                await foreach (var jsonBytes in ReadWebSocketMessagesAsync())
                {
                    if (jsonBytes == null || jsonBytes.Length == 0) continue;

                    if (jsonBytes.Length == 4 && jsonBytes[0] == (byte)'p' && jsonBytes[1] == (byte)'o' && jsonBytes[2] == (byte)'n' && jsonBytes[3] == (byte)'g')
                    {
                        //Console.WriteLine($"Get PONG {DateTime.Now:mm:ss} {id}");
                        timerWaitPong.Stop();
                        continue;
                    }
                    if (jsonBytes[0] != (byte)'{')
                    {
                        continue;
                    }
                    //string message2 = Encoding.UTF8.GetString(jsonBytes);
                    //Console.WriteLine(message2);
                    try
                    {
                        var root = new Utf8JsonReader(jsonBytes);
                        var parseObject = ParseMessage(ref root);
                        switch (parseObject.Item1)
                        {
                            case ParseObject.Data:
                                break;
                            case ParseObject.Ping:
                                //Console.WriteLine($"[{Settings.Print()}-{id}] Get PING {DateTime.Now:mm:ss}");
                                await SendPong(parseObject.Item2);
                                if (!Settings.CheckConnectByPing)
                                {
                                    timerWaitPong.Stop();
                                    timerWaitPong.Start();
                                }
                                break;
                            case ParseObject.Pong:
                                //Console.WriteLine($"[{Settings.Print()}-{id}] Get PONG {DateTime.Now:mm:ss}");
                                timerWaitPong.Stop();
                                break;
                            case ParseObject.Other:
                                //Console.WriteLine($"[{Settings.Print()}] {message}");
                                break;
                        }
                    }
                    catch (Exception ex)
                    {
                        string message = Encoding.UTF8.GetString(jsonBytes);
                        Console.WriteLine($"[{Name}] Ошибка маршрутизации сообщения: {ex.Message} {message}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[{Name}-ListenAsync] {DateTime.Now:hh:mm:ss} Ошибка ListenAsync: {ex.Message}");
                await ReConnect();
            }
        }

        protected abstract bool ParsePong(string value);
        protected abstract (ParseObject, object) ParseMessage(ref Utf8JsonReader reader);

        protected abstract object MessageSubscribeAll(string[] messages);
        protected abstract string MessageSubscribe(CurrencyPairResponceDto info);

        void StartPingTimer()
        {
            if (_pingTimer != null || !Settings.CheckConnectByPing) return;

            var intervalSend = Settings.IntervalPing * 1000;
            _pingTimer = new Timer(async _ =>
            {
                if (_webSocket.State == WebSocketState.Open)
                {
                    var ping = MessagePing();
                    await SendPing(ping);
                }
                else
                {
                    //Console.WriteLine("close");
                }

            }, null, intervalSend / 2, intervalSend);
        }
        protected virtual object MessagePing() => new { op = "ping" };
        async Task SendPing(object obj)
        {
            //Console.WriteLine($"[{Settings.Print()}-{id}] Send PING {DateTime.Now:mm:ss}");
            time_send_ping = DateTime.Now;

            var message = string.Empty;
            if (myParent.Info.Name == Exchanges.LBank && myParent.Info.Type == MarketType.Futures)
                message = (string)obj;
            else
                message = JsonConvert.SerializeObject(obj);
            try
            {
                Console.WriteLine(message);
                await _webSocket.SendAsync(
                   Encoding.UTF8.GetBytes(message),
                   WebSocketMessageType.Text,
                   true,
                   CancellationToken.None);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[{Name} SendPing] {ex.Message}");
            }
            timerWaitPong.Stop();
            timerWaitPong.Start();

        }
        async Task SendPong(object message)
        {
            //Console.WriteLine($"[{Settings.Print()}-{id}] Send PONG {DateTime.Now:mm:ss}");
            time_send_pong = DateTime.Now;

            var pong = JsonConvert.SerializeObject(message);
            try
            {
                Console.WriteLine(pong);
                await _webSocket.SendAsync(
                    Encoding.UTF8.GetBytes(pong),
                    WebSocketMessageType.Text,
                    true,
                    CancellationToken.None);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[{Name} SendPong]{ex.Message}");
            }
        }
    }

    public enum ParseObject
    {
        Data, Ping, Pong, Other
    }
}
