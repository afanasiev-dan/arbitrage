using Newtonsoft.Json;
using System.Text;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace Arbitrage.ExchangeDomain
{
    public class Network
    {
        /// <summary>
        /// Получение тела get запроса
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static async Task<string> GetAsync(string url)
        {
            using HttpClient httpClient = new HttpClient();
            HttpResponseMessage response = await httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }

        public static async Task<string> PostAsync(string url, object? jsonBody = null)
        {
            using HttpClient httpClient = new HttpClient();
            HttpContent? content = null;
            if (jsonBody != null)
            {
                string json = JsonConvert.SerializeObject(jsonBody);
                content = new StringContent(json, Encoding.UTF8, "application/json");
            }
            HttpResponseMessage response = await httpClient.PostAsync(url, content);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }
    }
}