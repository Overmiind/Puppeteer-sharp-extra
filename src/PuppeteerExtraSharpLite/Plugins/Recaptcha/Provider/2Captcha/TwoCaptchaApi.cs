using System.Net.Http;
using System.Net.Http.Json;

using PuppeteerExtraSharpLite.Plugins.Recaptcha.Provider._2Captcha.Models;

using System.Text;
using System.Text.Json;

namespace PuppeteerExtraSharpLite.Plugins.Recaptcha.Provider._2Captcha;

internal class TwoCaptchaApi {
    private static readonly Uri Host = new("https://rucaptcha.com");
    private readonly HttpClient _client;

    private readonly string _userKey;
    private readonly ProviderOptions _options;

    public TwoCaptchaApi(HttpClient client, string userKey, ProviderOptions options) {
        _client = client;
        _userKey = userKey;
        _options = options;
    }

    public async Task<TwoCaptchaResponse> CreateTaskAsync(string key, string pageUrl) {
        Uri uri = new(Host, "in.php");
        Dictionary<string, string> parameters = new() {
            ["key"] = _userKey,
            ["googlekey"] = key,
            ["pageurl"] = pageUrl,
            ["json"] = "1",
            ["method"] = "userrecaptcha"
        };
        string url = parameters.AddAsQueryTo(uri.AbsolutePath);

        using var message = new HttpRequestMessage(HttpMethod.Post, url);
        message.Headers.Add("Accept", "application/json");

        using var response = await _client.SendAsync(message);

        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync(JsonContext.Default.TwoCaptchaResponse)
        ?? throw new JsonException($"Failed to deserialize content into {nameof(TwoCaptchaResponse)}");
    }


    public async Task<TwoCaptchaResponse> GetSolution(string id) {
        // Build request URI with query parameters manually
        var sb = new StringBuilder("res.php");
        sb.Append('?');
        sb.Append("id=").Append(Uri.EscapeDataString(id));
        sb.Append('&').Append("key=").Append(Uri.EscapeDataString(_userKey));
        sb.Append('&').Append("action=get");
        sb.Append('&').Append("json=1");

        Uri uri = new(Host, sb.ToString());

        using var request = new HttpRequestMessage(HttpMethod.Post, uri);

        TwoCaptchaResponse? outerResult = null;

        await _client.SendPollingAsync(
            () => new HttpRequestMessage(HttpMethod.Post, uri),
            async response => {
                if (response.StatusCode != System.Net.HttpStatusCode.OK) {
                    return true;
                }

                var result = await response.Content.ReadFromJsonAsync(JsonContext.Default.TwoCaptchaResponse);

                if (result == null) {
                    return true;
                }

                if (result.request == "CAPCHA_NOT_READY") {
                    return true;
                }

                outerResult = result;
                return false;
            },
            _options.PendingCount,
            _options.StartTimeoutSeconds * 1000);

        return outerResult ?? new();
    }
}
