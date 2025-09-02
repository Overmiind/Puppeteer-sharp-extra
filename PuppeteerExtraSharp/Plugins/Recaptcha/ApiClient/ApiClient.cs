using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;
using PuppeteerExtraSharp.Utils;

namespace PuppeteerExtraSharp.Plugins.Recaptcha.ApiClient;

public class ApiClient
{
    private readonly HttpClient _client;

    public ApiClient(string baseUrl = null)
    {
        _client = new HttpClient();

        if (string.IsNullOrWhiteSpace(baseUrl)) return;

        _client.BaseAddress = new Uri(baseUrl);
    }

    public PollingRequest<T> CreatePostPollingRequest<T>(string url, object content)
    {
        return new PollingRequest<T>(_client, () =>
        {
            var request = new HttpRequestMessage(HttpMethod.Post, url);
            request.Content = JsonContent.Create(content);
            return request;
        });
    }

    public async Task<T?> PostAsync<T>(
        string url,
        object content,
        CancellationToken token = default)
    {
        var data = JsonContent.Create(content);
        var response = await _client.PostAsync(url, data, token);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<T>(cancellationToken: token);
    }

    private Uri CreateUri(string url, Dictionary<string, string> query)
    {
        if (!Uri.TryCreate(url, UriKind.Absolute, out var fullUri))
        {
            if (_client.BaseAddress == null)
                throw new InvalidOperationException("BaseAddress is not initialized");

            fullUri = new Uri(_client.BaseAddress, url);
        }

        var uriBuilder = new UriBuilder(fullUri)
        {
            Query = QueryHelper.ToQuery(query)
        };

        return uriBuilder.Uri;
    }
}