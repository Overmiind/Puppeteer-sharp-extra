using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;
namespace PuppeteerExtraSharp.Plugins.CaptchaSolver.ApiClient;

public class PollingRequest<T>(HttpClient client, Func<HttpRequestMessage> requestFactory)
{
    private const int DefaultPollIntervalSeconds = 5;
    private const int DefaultMaxAttempts = 5;

    private int _timeout = DefaultPollIntervalSeconds;
    private int _limit = DefaultMaxAttempts;

    public PollingRequest<T> WithTimeoutSeconds(int timeout)
    {
        if (timeout <= 0) throw new ArgumentOutOfRangeException(nameof(timeout), "Timeout must be positive.");
        _timeout = timeout;
        return this;
    }

    public PollingRequest<T> TriesLimit(int limit)
    {
        if (limit <= 0) throw new ArgumentOutOfRangeException(nameof(limit), "Limit must be positive.");
        _limit = limit;
        return this;
    }

    public async Task<ApiResponse<T>> ActivatePollingAsync(
        Func<ApiResponse<T>, PollingAction> decide, CancellationToken cancellation)
    {
        while (true)
        {
            cancellation.ThrowIfCancellationRequested();

            using var request = requestFactory();
            using var response = await client.SendAsync(request, cancellation).ConfigureAwait(false);

            var apiResponse = new ApiResponse<T>
            {
                Data = await response.Content.ReadFromJsonAsync<T>(cancellationToken: cancellation).ConfigureAwait(false),
                StatusCode = response.StatusCode
            };

            if (decide(apiResponse) == PollingAction.Break || _limit <= 1)
                return apiResponse;

            _limit -= 1;
            await Task.Delay(TimeSpan.FromSeconds(_timeout), cancellation).ConfigureAwait(false);
        }
    }
}

public class ApiResponse<T>
{
    public T? Data { get; set; }
    public System.Net.HttpStatusCode StatusCode { get; set; }
}

public enum PollingAction
{
    ContinuePolling,
    Break
}