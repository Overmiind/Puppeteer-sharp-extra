using System.Net.Http;

namespace PuppeteerExtraSharpLite.Plugins.Recaptcha.Provider.AntiCaptcha;

public class AntiCaptcha : IRecaptchaProvider {
    private readonly ProviderOptions _options;

    private readonly AntiCaptchaApi _api;

    public AntiCaptcha(HttpClient client, string userKey, ProviderOptions? options = null) {
        _options = options ?? ProviderOptions.CreateDefaultOptions();
        _api = new AntiCaptchaApi(client, userKey, _options);
    }

    public async Task<string> GetSolution(string key, string pageUrl, string proxyStr = "") {
        var task = await _api.CreateTaskAsync(pageUrl, key);
        await Task.Delay(_options.StartTimeoutSeconds * 1000);
        var result = await _api.PendingForResult(task.taskId);

        if (result.status != "ready" || result.solution is null || result.errorId != 0) {
            throw new HttpRequestException($"AntiCaptcha request ends with error - {result.errorId}");
        }

        return result.solution.gRecaptchaResponse;
    }
}
