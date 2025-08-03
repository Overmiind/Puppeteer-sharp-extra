using System.Net.Http;
using System.Net.Http.Json;

using PuppeteerExtraSharpLite.Plugins.Recaptcha.Provider._2Captcha.Models;
using PuppeteerExtraSharpLite.Plugins.Recaptcha.RestClient;

using RestSharp;

namespace PuppeteerExtraSharpLite.Plugins.Recaptcha.Provider._2Captcha;

internal class TwoCaptchaApi {
    private readonly RestClient.RestClient _client = new("https://rucaptcha.com");
    private readonly string _userKey;
    private readonly ProviderOptions _options;

    public TwoCaptchaApi(string userKey, ProviderOptions options) {
        _userKey = userKey;
        _options = options;
    }

    public async Task<TwoCaptchaResponse> CreateTaskAsync(string key, string pageUrl) {
        var result = await _client.PostWithQueryAsync("in.php", new Dictionary<string, string>() {
            ["key"] = _userKey,
            ["googlekey"] = key,
            ["pageurl"] = pageUrl,
            ["json"] = "1",
            ["method"] = "userrecaptcha"
        }, JsonContext.Default.TwoCaptchaResponse);

        return result;
    }


    public async Task<TwoCaptchaResponse> GetSolution(string id) {
        using var request = new HttpRequestMessage(HttpMethod.Post, "res.php");

        request.AddQueryParameter("id", id);
        request.AddQueryParameter("key", _userKey);
        request.AddQueryParameter("action", "get");
        request.AddQueryParameter("json", "1");

        TwoCaptchaResponse? outerResult = null;

        var result = await _client.CreatePollingBuilder<TwoCaptchaResponse>(request).TriesLimit(_options.PendingCount).ActivatePollingAsync(
            async response => {
                if (response.StatusCode != System.Net.HttpStatusCode.OK) {
                    return PollingAction.ContinuePolling;
                }

                var result = await response.Content.ReadFromJsonAsync(JsonContext.Default.TwoCaptchaResponse);

                if (result == null) {
                    return PollingAction.ContinuePolling;
                }

                if (result.request == "CAPCHA_NOT_READY") {
                    return PollingAction.ContinuePolling;
                }

                outerResult = result;
                return PollingAction.Break;
            });

        return outerResult ?? new(); ;
    }
}
