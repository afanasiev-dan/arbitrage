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

        public static async Task<string> PostAsync(string url)
        {
            using HttpClient httpClient = new HttpClient();
            HttpResponseMessage response = await httpClient.PostAsync(url, null);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }
    }
}