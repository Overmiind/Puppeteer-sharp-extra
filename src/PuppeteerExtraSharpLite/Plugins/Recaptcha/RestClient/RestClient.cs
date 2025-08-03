using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json.Serialization.Metadata;

namespace PuppeteerExtraSharpLite.Plugins.Recaptcha.RestClient;

public sealed class RestClient : IDisposable {
    private readonly HttpClient _client;
    private bool _disposed;

    public RestClient(string url = "") {
        _client = string.IsNullOrWhiteSpace(url) ? new HttpClient() : new HttpClient() {
            BaseAddress = new Uri(url)
        };
    }

    public PollingBuilder<T> CreatePollingBuilder<T>(HttpRequestMessage request) {
        return new PollingBuilder<T>(_client, request);
    }

    public void Dispose() {
        if (_disposed) {
            return;
        }
        _client.Dispose();
        _disposed = true;
    }

    public async Task<TOutput?> PostWithJsonAsync<TInput, TOutput>(string url, TInput content, JsonTypeInfo<TInput> inputTypeInfo, JsonTypeInfo<TOutput> outputTypeInfo, CancellationToken token) {
        var request = new HttpRequestMessage();
        request.Headers.Add("Accept", "application/json");
        request.Method = HttpMethod.Post;
        request.Content = JsonContent.Create(content, inputTypeInfo);
        request.RequestUri = new Uri(url);
        var response = await _client.SendAsync(request, token);
        return await response.Content.ReadFromJsonAsync(outputTypeInfo, token);
    }

    public async Task<TOutput> PostWithQueryAsync<TOutput>(string url, Dictionary<string, string> query, JsonTypeInfo<TOutput> outputTypeInfo, CancellationToken token = default) {
        // Build full URL with query parameters if any using StringBuilder (no LINQ)
        var fullUrl = url;
        if (query != null && query.Count > 0) {
            var sb = new StringBuilder(url);
            var separator = url.Contains('?') ? '&' : '?';
            sb.Append(separator);
            var first = true;
            foreach (var kvp in query) {
                if (!first) {
                    sb.Append('&');
                } else {
                    first = false;
                }
                sb.Append(Uri.EscapeDataString(kvp.Key));
                sb.Append('=');
                sb.Append(Uri.EscapeDataString(kvp.Value));
            }
            fullUrl = sb.ToString();
        }
        var request = new HttpRequestMessage(HttpMethod.Post, fullUrl);
        request.Headers.Add("Accept", "application/json");

        var response = await _client.SendAsync(request, token);
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync(outputTypeInfo, cancellationToken: token);
        return result ?? throw new InvalidOperationException("Failed to deserialize response.");
    }
}