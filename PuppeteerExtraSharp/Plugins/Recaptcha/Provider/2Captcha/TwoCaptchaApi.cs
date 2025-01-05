using System.Collections.Generic;
using System.Threading.Tasks;
using PuppeteerExtraSharp.Plugins.Recaptcha.Provider._2Captcha.Models;
using PuppeteerExtraSharp.Plugins.Recaptcha.RestClient;
using RestSharp;

namespace PuppeteerExtraSharp.Plugins.Recaptcha.Provider._2Captcha;

internal class TwoCaptchaApi
{
    private readonly RestClient.RestClient _client = new("https://rucaptcha.com");

    private readonly string _userKey;
    private readonly ProviderOptions _options;

    public TwoCaptchaApi(string userKey, ProviderOptions options)
    {
        _userKey = userKey;
        _options = options;
    }

    public async Task<TwoCaptchaResponse> CreateTaskAsync(string key, string pageUrl)
    {
        var result = await _client.PostWithQueryAsync<TwoCaptchaResponse>(
            "in.php",
            new Dictionary<string, string>
            {
                ["key"] = _userKey,
                ["googlekey"] = key,
                ["pageurl"] = pageUrl,
                ["json"] = "1",
                ["method"] = "userrecaptcha"
            });

        return result;
    }

    public async Task<RestResponse<TwoCaptchaResponse>> GetSolution(string id)
    {
        var request = new RestRequest("res.php") { Method = Method.Post };

        request.AddQueryParameter("id", id);
        request.AddQueryParameter("key", _userKey);
        request.AddQueryParameter("action", "get");
        request.AddQueryParameter("json", "1");

        var result = await _client.CreatePollingBuilder<TwoCaptchaResponse>(request)
            .TriesLimit(_options.PendingCount).ActivatePollingAsync(
                response => response.Data.request == "CAPCHA_NOT_READY"
                    ? PollingAction.ContinuePolling
                    : PollingAction.Break);

        return result;
    }
}
