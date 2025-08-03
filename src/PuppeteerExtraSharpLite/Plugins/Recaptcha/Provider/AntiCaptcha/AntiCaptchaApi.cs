using System.Net.Http;
using System.Net.Http.Json;

using PuppeteerExtraSharpLite.Plugins.Recaptcha.Provider.AntiCaptcha.Models;
using PuppeteerExtraSharpLite.Plugins.Recaptcha.RestClient;

namespace PuppeteerExtraSharpLite.Plugins.Recaptcha.Provider.AntiCaptcha;

public class AntiCaptchaApi : IDisposable {
    private bool _disposed;

    private readonly string _userKey;
    private readonly ProviderOptions _options;
    private readonly RestClient.RestClient _client = new("http://api.anti-captcha.com");
    public AntiCaptchaApi(string userKey, ProviderOptions options) {
        _userKey = userKey;
        _options = options;
    }

    public Task<AntiCaptchaTaskResult> CreateTaskAsync(string pageUrl, string key, CancellationToken token = default) {
        var content = new AntiCaptchaRequest() {
            clientKey = _userKey,
            task = new AntiCaptchaTask() {
                type = "NoCaptchaTaskProxyless",
                websiteURL = pageUrl,
                websiteKey = key
            }
        };

        var result = _client.PostWithJsonAsync("createTask",
                                                                     content,
                                                                     JsonContext.Default.AntiCaptchaRequest,
                                                                     JsonContext.Default.AntiCaptchaTaskResult,
                                                                     token);
        return result;
    }

    public void Dispose() {
        if (_disposed) {
            return;
        }
        _client.Dispose();
        _disposed = true;
    }

    public async Task<TaskResultModel> PendingForResult(int taskId, CancellationToken token = default) {
        var content = new RequestForResultTask() {
            clientKey = _userKey,
            taskId = taskId
        };


        var request = new HttpRequestMessage(HttpMethod.Post, "getTaskResult");
        request.Content = JsonContent.Create(content, JsonContext.Default.RequestForResultTask);

        TaskResultModel? outerResult = null;

        await _client.CreatePollingBuilder<TaskResultModel>(request).TriesLimit(_options.PendingCount)
            .WithTimeoutSeconds(5).ActivatePollingAsync(
                async response => {
                    if (response.StatusCode != System.Net.HttpStatusCode.OK) {
                        return PollingAction.ContinuePolling;
                    }

                    var result = await response.Content.ReadFromJsonAsync(JsonContext.Default.TaskResultModel);

                    if (result is null) {
                        return PollingAction.ContinuePolling;
                    }

                    if (result.status == "ready" || result.errorId != 0) {
                        outerResult = result;
                        return PollingAction.Break;
                    }

                    return PollingAction.ContinuePolling;
                });
        return outerResult ?? new();
    }
}
