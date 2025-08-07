using System.Net.Http;
using System.Net.Http.Json;

using PuppeteerExtraSharpLite.Plugins.Recaptcha.Provider.AntiCaptcha.Models;

namespace PuppeteerExtraSharpLite.Plugins.Recaptcha.Provider.AntiCaptcha;

public class AntiCaptcha : IRecaptchaProvider {
    private readonly HttpClient _client;
    private readonly string _userKey;
    private readonly ProviderOptions _options;

    public AntiCaptcha(HttpClient client, string userKey) : this(client, userKey, ProviderOptions.Default) { }

    public AntiCaptcha(HttpClient client, string userKey, ProviderOptions options) {
        _client = client;
        _userKey = userKey;
        _options = options;
    }

    public async Task<string> GetSolution(string key, string pageUrl, string proxyStr = "") {
        var task = await Api.CreateTaskAsync(_client, _userKey, pageUrl, key);
        await Task.Delay(_options.StartTimeoutSeconds * 1000);
        var result = await Api.PendingForResult(_client, _userKey, task.TaskId, _options);

        if (result.status != "ready" || result.solution is null || result.errorId != 0) {
            throw new HttpRequestException($"AntiCaptcha request ends with error - {result.errorId}");
        }

        return result.solution.gRecaptchaResponse;
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

            return await response.Content.ReadFromJsonAsync(JsonContext.Default.AntiCaptchaTaskResult, cancellationToken: token);
        }

        public static async Task<TaskResultModel> PendingForResult(HttpClient client, string userKey, int taskId, ProviderOptions options, CancellationToken token = default) {
            var content = new RequestForResultTask() {
                ClientKey = userKey,
                TaskId = taskId
            };

            Uri uri = new(Host, "getTaskResult");

            TaskResultModel? outerResult = null;

            await client.SendPollingAsync(
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
                    options.PendingCount,
                    options.StartTimeoutSeconds * 1000);

            return outerResult ?? new();
        }
    }
}
