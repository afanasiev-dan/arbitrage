using System;
using System.Collections.Concurrent;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

public static class SmartHttp
{
    private static readonly SemaphoreSlim _semaphore = new(5); // максимум 5 одновременных запросов
    private static readonly ConcurrentQueue<Func<Task>> _queue = new();
    private static readonly object _lock = new();
    private static bool _isRunning = false;

    public static Task<string> GetAsync(string url, int timeout = 3000)
    {
        var tcs = new TaskCompletionSource<string>();

        async Task TaskWrapper()
        {
            _ = Task.Run(async () =>
            {
                await _semaphore.WaitAsync();
                try
                {
                    string result = await PerformRequestWithRetries(url, timeout);
                    tcs.TrySetResult(result);
                }
                catch (Exception ex)
                {
                    tcs.TrySetException(ex);
                }
                finally
                {
                    _semaphore.Release();
                    RunNext();
                }
            });
        }

        lock (_lock)
        {
            _queue.Enqueue(TaskWrapper);
            if (!_isRunning)
            {
                _isRunning = true;
                RunNext();
            }
        }

        return tcs.Task;
    }

    private static void RunNext()
    {
        lock (_lock)
        {
            if (_queue.TryDequeue(out var next))
            {
                next();
            }
            else
            {
                _isRunning = false;
            }
        }
    }

    private static async Task<string> PerformRequestWithRetries(string url, int timeout, int attempt = 0)
    {
        using (HttpClient httpClient = new HttpClient())
        {
            httpClient.Timeout = TimeSpan.FromMilliseconds(timeout);
            try
            {
                var response = await httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadAsStringAsync();
            }
            catch (Exception ex) when (attempt < 3)
            {
                await Task.Delay(1000);
                return await PerformRequestWithRetries(url, timeout, attempt + 1);
            }
            catch (Exception e)
            {
                Console.WriteLine($"[FAIL] {e.Message} - {url}");
                return e.Message;
            }
        }
    }
}
