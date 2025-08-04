using System.Net.Http;
using System.Net.Http.Json;

using PuppeteerExtraSharpLite.Plugins.Recaptcha.Provider.AntiCaptcha.Models;

namespace PuppeteerExtraSharpLite.Plugins.Recaptcha.Provider.AntiCaptcha;

public class AntiCaptchaApi {
    private static readonly Uri Host = new("http://api.anti-captcha.com");

    private readonly string _userKey;
    private readonly ProviderOptions _options;
    private readonly HttpClient _client;
    public AntiCaptchaApi(HttpClient client, string userKey, ProviderOptions options) {
        _userKey = userKey;
        _options = options;
        _client = client;
    }

    public async Task<AntiCaptchaTaskResult> CreateTaskAsync(string pageUrl, string key, CancellationToken token = default) {
        var content = new AntiCaptchaRequest() {
            clientKey = _userKey,
            task = new AntiCaptchaTask() {
                type = "NoCaptchaTaskProxyless",
                websiteURL = pageUrl,
                websiteKey = key
            }
        };

        Uri uri = new(Host, "createTask");
        using var message = new HttpRequestMessage(HttpMethod.Post, uri);
        message.Headers.Add("Accept", "application/json");
        message.Content = JsonContent.Create(content, JsonContext.Default.AntiCaptchaRequest);

        using var response = await _client.SendAsync(message, token);
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync(JsonContext.Default.AntiCaptchaTaskResult, cancellationToken: token);
    }

    public async Task<TaskResultModel> PendingForResult(int taskId, CancellationToken token = default) {
        var content = new RequestForResultTask() {
            clientKey = _userKey,
            taskId = taskId
        };

        Uri uri = new(Host, "getTaskResult");

        TaskResultModel? outerResult = null;

        await _client.SendPollingAsync(
                () => {
                    var message = new HttpRequestMessage(HttpMethod.Post, uri);
                    message.Headers.Add("Accept", "application/json");
                    message.Content = JsonContent.Create(content, JsonContext.Default.RequestForResultTask);
                    return message;
                },
                async response => {
                    if (response.StatusCode != System.Net.HttpStatusCode.OK) {
                        return true;
                    }

                    var result = await response.Content.ReadFromJsonAsync(JsonContext.Default.TaskResultModel);

                    if (result is null) {
                        return true;
                    }

                    if (result.status == "ready" || result.errorId != 0) {
                        outerResult = result;
                        return false;
                    }

                    return true;
                },
                _options.PendingCount,
                _options.StartTimeoutSeconds * 1000);

        return outerResult ?? new();
    }
}
