using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json.Serialization.Metadata;

using PuppeteerExtraSharpLite.Utils;

using RestSharp;

namespace PuppeteerExtraSharpLite.Plugins.Recaptcha.RestClient;

public sealed class RestClient : IDisposable {
    private readonly HttpClient _client;
    private bool _disposed;

    public RestClient(string url = "") {
        _client = string.IsNullOrWhiteSpace(url) ? new HttpClient() : new HttpClient() {
            BaseAddress = new Uri(url)
        };
    }

    public PollingBuilder<T> CreatePollingBuilder<T>(RestRequest request) {
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

    public async Task<T> PostWithQueryAsync<T>(string url, Dictionary<string, string> query, CancellationToken token = default) {
        // var request = new RestRequest(url) { Method = Method.Post };

        // request.AddQueryParameters(query);
        // return await _client.PostAsync<T>(request, token);
        var request = new HttpRequestMessage(HttpMethod.Post, url);
        // request.RequestUri.
    }
}