using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

using PuppeteerExtraSharpLite.Plugins.Recaptcha.Provider._2Captcha.Models;

namespace PuppeteerExtraSharpLite.Plugins.Recaptcha.Provider._2Captcha;

public class TwoCaptcha : IRecaptchaProvider {
    private readonly HttpClient _client;
    private readonly string _userKey;
    private readonly ProviderOptions _options;

    public TwoCaptcha(HttpClient client, string userKey) : this(client, userKey, ProviderOptions.Default) { }

    public TwoCaptcha(HttpClient client, string userKey, ProviderOptions options) {
        _client = client;
        _userKey = userKey;
        _options = options;
    }

    public async Task<string> GetSolution(string key, string pageUrl, string proxyStr = "") {
        var task = await Api.CreateTaskAsync(_client, _userKey, key, pageUrl);

        ThrowErrorIfBadStatus(task);

        await Task.Delay(_options.StartTimeoutSeconds * 1000);

        var result = await Api.GetSolution(_client, _userKey, task.Request, _options);

        ThrowErrorIfBadStatus(result);

        return result.Request;
    }

    private static void ThrowErrorIfBadStatus(TwoCaptchaResponse response) {
        if (response.Status != 1 || string.IsNullOrEmpty(response.Request)) {
            throw new HttpRequestException($"Two captcha request ends with error [{response.Status}] {response.Request}");
        }
    }

    public static class Api {
        private static readonly Uri Host = new("https://rucaptcha.com");

        public static async Task<TwoCaptchaResponse> CreateTaskAsync(HttpClient client, string userKey, string key, string pageUrl, CancellationToken token = default) {
            Uri uri = new(Host, "in.php");
            Dictionary<string, string> parameters = new() {
                ["key"] = userKey,
                ["googlekey"] = key,
                ["pageurl"] = pageUrl,
                ["json"] = "1",
                ["method"] = "userrecaptcha"
            };
            string url = parameters.AddAsQueryTo(uri.AbsolutePath);

            using var message = new HttpRequestMessage(HttpMethod.Post, url);
            message.Headers.Add("Accept", "application/json");

            using var response = await client.SendAsync(message, token);

            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync(JsonContext.Default.TwoCaptchaResponse, cancellationToken: token)
            ?? throw new JsonException($"Failed to deserialize content into {nameof(TwoCaptchaResponse)}");
        }


        public static async Task<TwoCaptchaResponse> GetSolution(HttpClient client, string userKey, string id, ProviderOptions options, CancellationToken token = default) {
            // Build request URI with query parameters manually
            var sb = new StringBuilder("res.php");
            sb.Append('?');
            sb.Append("id=").Append(Uri.EscapeDataString(id));
            sb.Append('&').Append("key=").Append(Uri.EscapeDataString(userKey));
            sb.Append('&').Append("action=get");
            sb.Append('&').Append("json=1");

            Uri uri = new(Host, sb.ToString());

            using var request = new HttpRequestMessage(HttpMethod.Post, uri);

            TwoCaptchaResponse? outerResult = null;

            await client.SendPollingAsync(
                () => new HttpRequestMessage(HttpMethod.Post, uri),
                async response => {
                    if (response.StatusCode != System.Net.HttpStatusCode.OK) {
                        return true;
                    }

                    var result = await response.Content.ReadFromJsonAsync(JsonContext.Default.TwoCaptchaResponse);

                    if (result == null) {
                        return true;
                    }

                    if (result.Request == "CAPCHA_NOT_READY") {
                        return true;
                    }

                    outerResult = result;
                    return false;
                },
                options.PendingCount,
                options.StartTimeoutSeconds * 1000);

            return outerResult ?? new();
        }
    }
}
