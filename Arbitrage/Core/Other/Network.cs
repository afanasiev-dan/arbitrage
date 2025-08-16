using Arbitrage.Core.Base.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Arbitrage.Other
{
    public class Network
    {
        private static readonly HttpClient httpClient = new HttpClient();

        public static async Task<string> GetAsync(string url, int timeOut = 10000, int k = 0)
        {
            try
            {
                //using var cts = new CancellationTokenSource(timeOut);
                HttpResponseMessage response = await httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();
                return responseBody;
            }
            catch (TaskCanceledException ex) when (k < 3)
            {
                await Task.Delay(2000);
                return await GetAsync(url, timeOut, ++k);
            }
            catch (Exception e)
            {
                Console.WriteLine($"GetAsync: {e.Message} {url}");
                return e.Message;
            }
        }
        public static async Task<string> PostAsync(string url, object content, Dictionary<string, string> headers = null, int timeOut = 10000, int k = 0)
        {
            try
            {
                var jsonContent = new StringContent(
                    JsonSerializer.Serialize(content),
                    Encoding.UTF8,
                    "application/json"
                );

                var request = new HttpRequestMessage(HttpMethod.Post, url)
                {
                    Content = jsonContent
                };

                if (headers != null)
                {
                    foreach (var header in headers)
                    {
                        request.Headers.TryAddWithoutValidation(header.Key, header.Value);
                    }
                }
                HttpResponseMessage response = await httpClient.SendAsync(request);
                response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();
                return responseBody;
            }
            catch (TaskCanceledException ex) when (k < 3)
            {
                await Task.Delay(2000);
                return await PostAsync(url, content, headers, timeOut, ++k);
            }
            catch (Exception e)
            {
                Console.WriteLine($"{e.Message} {url}");
                return e.Message;
            }
        }
        public static string GetUrl(string coin, ExchangeEnum exchange, AssetTypeEnum assetType)
        {
            string url = "http://";
            switch(exchange)
            {
                case ExchangeEnum.ByBit:
                    url += "www.bybit.com/";
                    switch(assetType)
                    {
                        case AssetTypeEnum.Futures:
                            //https://www.bybit.com/trade/usdt/BTCUSDT
                            url += $"trade/usdt/{coin}USDT";
                            break;
                        case AssetTypeEnum.Spot:
                            //https://www.bybit.com/ru-RU/trade/spot/BTC/USDT
                            url += $"ru-RU/trade/spot/{coin}/USDT";
                            break;
                    }
                    break;
                case ExchangeEnum.KuCoin:
                    url += "www.kucoin.com/";
                    switch (assetType)
                    {
                        case AssetTypeEnum.Spot:
                            //https://www.kucoin.com/ru/trade/BTC-USDT
                            url += $"ru/trade/{coin}-USDT";
                            break;
                        case AssetTypeEnum.Futures:
                            //https://www.kucoin.com/ru/futures/trade/XBTUSDTM
                            url += $"ru/futures/trade/{coin}USDTM";
                            break;
                    }
                    break;
                case ExchangeEnum.Gateio:
                    url += "www.gate.io/";
                    switch (assetType)
                    {
                        case AssetTypeEnum.Spot:
                            //https://www.gate.io/ru/trade/BTC_USDT
                            url += $"ru/trade/{coin}_USDT";
                            break;
                        case AssetTypeEnum.Futures:
                            //https://www.gate.io/ru/futures/USDT/BTC_USDT
                            url += $"ru/futures/USDT/{coin}_USDT";
                            break;
                    }
                    break;
                case ExchangeEnum.HTX:
                    url += "www.htx.com/";
                    switch (assetType)
                    {
                        case AssetTypeEnum.Spot:
                            //https://www.htx.com/ru-ru/trade/btc_usdt?type=spot
                            url += $"ru-ru/trade/{coin.ToLower()}_usdt?type=spot";
                            break;
                        case AssetTypeEnum.Futures:
                            //https://www.htx.com/ru-ru/futures/linear_swap/exchange#contract_code=BTC-USDT&contract_type=swap&type=cross
                            url += $"ru-ru/futures/linear_swap/exchange#contract_code={coin.ToUpper()}-USDT&contract_type=swap&type=cross";
                            break;
                    }
                    break;
                case ExchangeEnum.Mexc:
                    url += "www.mexc.com/";
                    switch (assetType)
                    {
                        case AssetTypeEnum.Spot:
                            //https://www.mexc.com/ru-RU/exchange/BTC_USDT
                            url += $"ru-RU/exchange/{coin}_USDT";
                            break;
                        case AssetTypeEnum.Futures:
                            //https://www.mexc.com/ru-RU/futures/BTC_USDT?type=linear_swap&lang=ru-RU
                            url += $"ru-RU/futures/{coin}_USDT?type=linear_swap&lang=ru-RU";
                            break;
                    }
                    break;
                case ExchangeEnum.LBank:
                    url += "www.lbank.com/";
                    switch (assetType)
                    {
                        case AssetTypeEnum.Spot:
                            //https://www.lbank.com/ru/trade/btc_usdt
                            url += $"ru/trade/{coin.ToLower()}_usdt";
                            break;
                        case AssetTypeEnum.Futures:
                            //https://www.lbank.com/ru/futures/btcusdt
                            url += $"ru/futures/{coin.ToLower()}usdt";
                            break;
                    }
                    break;
            }
            return url;
        }


        //https://graphs.arbitrage-services.com/?
        //type=spot&
        //long=bingx&
        //short=mexc&
        //coin=VVAIFUUSDT
        public static string GetUrlArbService(AssetTypeEnum typeEnum, ExchangeEnum longEnum, ExchangeEnum shortEnum, string coin)
        {
            string url = "https://graphs.arbitrage-services.com/?";
            switch (typeEnum)
            {
                case AssetTypeEnum.Spot:
                    url += "type=spot&";
                    break;
                case AssetTypeEnum.Futures:
                    url += "type=futures&";
                    break;
            }
            url += $"long={GetNameExchange(longEnum)}&";
            url += $"short={GetNameExchange(shortEnum)}&";
            url += $"coin={coin}USDT";
            return url;
        }

        static string GetNameExchange(ExchangeEnum ex)
        {
            string name = ex.ToString().ToLower();
            if (name == "htx")
                name = "huobi";
            return name;
        }
    }
    public static class HttpClientExtensions
    {
        public static async Task<string> Get(this HttpClient httpClient, string url, int timeout = 3000, int maxRetries = 3)
        {
            return await GetInternalAsync(httpClient, url, timeout, maxRetries);
        }

        private static async Task<string> GetInternalAsync(HttpClient httpClient, string url, int timeout, int retriesLeft)
        {
            try
            {
                //using var cts = new CancellationTokenSource(timeout);
                var response = await httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadAsStringAsync();
            }
            catch (TaskCanceledException) when (retriesLeft > 0)
            {
                await Task.Delay(2000);
                return await GetInternalAsync(httpClient, url, timeout, retriesLeft - 1);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{ex.Message} | {url}");
                return ex.Message;
            }
        }
    }

}