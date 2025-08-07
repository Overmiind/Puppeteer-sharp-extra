using System.Net.Http;
using System.Net.Http.Json;

namespace PuppeteerExtraSharpLite.Plugins.Recaptcha.Provider.AntiCaptcha;

public class AntiCaptchaProvider : IRecaptchaProvider {
    private readonly HttpClient _client;
    private readonly string _userKey;
    private readonly ProviderOptions _options;

    public AntiCaptchaProvider(HttpClient client, string userKey) : this(client, userKey, ProviderOptions.Default) { }

    public AntiCaptchaProvider(HttpClient client, string userKey, ProviderOptions options) {
        _client = client;
        _userKey = userKey;
        _options = options;
    }

    public async Task<string> GetSolution(string key, string pageUrl, string proxyStr = "") {
        var task = await Api.CreateTaskAsync(_client, _userKey, pageUrl, key);
        await Task.Delay(_options.StartTimeoutSeconds * 1000);
        var result = await Api.PendingForResult(_client, _userKey, task.TaskId, _options);

        if (result.Status != "ready" || result.Solution is null || result.ErrorId != 0) {
            throw new HttpRequestException($"AntiCaptcha request ends with error - {result.ErrorId}");
        }

        return result.Solution.GRecaptchaResponse;
    }

    public static class Api {
        private static readonly Uri Host = new("http://api.anti-captcha.com");

        public static async Task<AntiCaptchaTaskResult> CreateTaskAsync(HttpClient client, string userKey, string pageUrl, string key, CancellationToken token = default) {
            var content = new AntiCaptchaRequest() {
                ClientKey = userKey,
                Task = new AntiCaptchaTask() {
                    Type = "NoCaptchaTaskProxyless",
                    WebsiteURL = pageUrl,
                    WebsiteKey = key
                }
            };

            Uri uri = new(Host, "createTask");
            using var message = new HttpRequestMessage(HttpMethod.Post, uri);
            message.Headers.Add("Accept", "application/json");
            message.Content = JsonContent.Create(content, JsonContext.Default.AntiCaptchaRequest);

            using var response = await client.SendAsync(message, token);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync(JsonContext.Default.AntiCaptchaTaskResult, cancellationToken: token) ?? new();
        }

        public static async Task<AntiCaptchaTaskResultModel> PendingForResult(HttpClient client, string userKey, int taskId, ProviderOptions options, CancellationToken token = default) {
            var content = new AntiCaptchaRequestForResultTask() {
                ClientKey = userKey,
                TaskId = taskId
            };

            Uri uri = new(Host, "getTaskResult");

            AntiCaptchaTaskResultModel outerResult = new();

            await client.SendPollingAsync(
                    () => {
                        var message = new HttpRequestMessage(HttpMethod.Post, uri);
                        message.Headers.Add("Accept", "application/json");
                        message.Content = JsonContent.Create(content, JsonContext.Default.AntiCaptchaRequestForResultTask);
                        return message;
                    },
                    async response => {
                        if (response.StatusCode != System.Net.HttpStatusCode.OK) {
                            return true;
                        }

                        var result = await response.Content.ReadFromJsonAsync(JsonContext.Default.AntiCaptchaTaskResultModel);

                        if (result is null) {
                            return true;
                        }

                        if (result.Status == "ready" || result.ErrorId != 0) {
                            outerResult = result;
                            return false;
                        }

                        return true;
                    },
                    options.PendingCount,
                    options.StartTimeoutSeconds * 1000);

            return outerResult;
        }
    }
}
