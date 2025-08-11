/*

//TODO: Disabled pending "GET RECAPTCHA KEY" review

using System.Net.Http;
using System.Net.Http.Json;

namespace PuppeteerSharpToolkit.Plugins.Recaptcha.AntiCaptcha;

/// <summary>
/// Anti-Captcha provider implementation that requests and polls for reCAPTCHA tokens
/// using the Anti-Captcha public HTTP API.
/// </summary>
public class AntiCaptchaProvider : IRecaptchaProvider {
    private readonly HttpClient _client;
    private readonly string _userKey;
    private readonly ProviderOptions _options;

    /// <summary>
    /// Initializes the provider with default polling options.
    /// </summary>
    /// <param name="client">HTTP client used for API calls.</param>
    /// <param name="userKey">Anti-Captcha account client key.</param>
    public AntiCaptchaProvider(HttpClient client, string userKey) : this(client, userKey, ProviderOptions.Default) { }

    /// <summary>
    /// Initializes the provider with custom options.
    /// </summary>
    /// <param name="client">HTTP client used for API calls.</param>
    /// <param name="userKey">Anti-Captcha account client key.</param>
    /// <param name="options">Polling options.</param>
    public AntiCaptchaProvider(HttpClient client, string userKey, ProviderOptions options) {
        _client = client;
        _userKey = userKey;
        _options = options;
    }

    /// <summary>
    /// Creates a task for the target page and polls Anti-Captcha until a token is ready.
    /// </summary>
    /// <param name="pageUrl">URL containing the widget.</param>
    /// <param name="proxyStr">Optional proxy string (unused in proxyless task).</param>
    /// <param name="token">Cancellation token.</param>
    /// <returns>Solution token received from Anti-Captcha.</returns>
    /// <exception cref="HttpRequestException">Thrown when the API returns an error or invalid result.</exception>
    public async Task<string> GetSolutionAsync(string pageUrl, string proxyStr = "", CancellationToken token = default) {
        var task = await Api.CreateTaskAsync(_client, _userKey, pageUrl, token).ConfigureAwait(false);
        await Task.Delay(_options.StartTimeoutSeconds * 1000, token);
        var result = await Api.PendingForResult(_client, _userKey, task.TaskId, _options, token).ConfigureAwait(false);

        if (result.Status != "ready" || result.Solution is null || result.ErrorId != 0) {
            throw new HttpRequestException($"AntiCaptcha request ends with error - {result.ErrorId} - {result.ErrorCode} - {result.ErrorDescription}");
        }

        return result.Solution.GRecaptchaResponse;
    }

    /// <summary>
    /// Internal Anti-Captcha API wrappers.
    /// </summary>
    public static class Api {
        private static readonly Uri Host = new("http://api.anti-captcha.com");

        /// <summary>
        /// Creates a new Anti-Captcha task for the specified page and site key.
        /// </summary>
        /// <param name="client">HTTP client.</param>
        /// <param name="userKey">Client key for Anti-Captcha.</param>
        /// <param name="pageUrl">Target page URL.</param>
        /// <param name="token">Cancellation token.</param>
        /// <returns>Task creation response.</returns>
        public static async Task<AntiCaptchaTaskResult> CreateTaskAsync(HttpClient client, string userKey, string pageUrl, CancellationToken token = default) {
            Uri uri = new(Host, "createTask");
            using var message = new HttpRequestMessage(HttpMethod.Post, uri);
            message.Headers.Add("Accept", "application/json");
            message.Content = new StringContent(
                $$"""
                {
                    "clientKey": "{{userKey}}",
                    "task":
                    {
                        "type": "NoCaptchaTaskProxyless",
                        "websiteURL": "{{pageUrl}}"
                    }
                }
                """
            );

            using var response = await client.SendAsync(message, token).ConfigureAwait(false);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync(JsonContext.Default.AntiCaptchaTaskResult, cancellationToken: token)
            .ConfigureAwait(false) ?? new AntiCaptchaTaskResult();
        }

        /// <summary>
        /// Polls Anti-Captcha for task completion and returns the final result or error.
        /// </summary>
        /// <param name="client">HTTP client.</param>
        /// <param name="userKey">Client key for Anti-Captcha.</param>
        /// <param name="taskId">Identifier of the created task.</param>
        /// <param name="options">Polling options.</param>
        /// <param name="token">Cancellation token.</param>
        /// <returns>Task result model; <c>Status</c> is <c>ready</c> when solved.</returns>
        public static async Task<AntiCaptchaTaskResultFull> PendingForResult(HttpClient client, string userKey, int taskId, ProviderOptions options, CancellationToken token = default) {
            string content =
            $$"""
            {
                "clientKey": "{{userKey}}",
                "taskId": {{taskId}}
            }
            """;

            Uri uri = new(Host, "getTaskResult");

            AntiCaptchaTaskResultFull outerResult = new();

            await client.SendPollingAsync(
                    () => {
                        var message = new HttpRequestMessage(HttpMethod.Post, uri);
                        message.Headers.Add("Accept", "application/json");
                        message.Content = new StringContent(content);
                        return message;
                    },
                    (Func<HttpResponseMessage, Task<bool>>)(async response => {
                        if (response.StatusCode != System.Net.HttpStatusCode.OK) {
                            return true;
                        }

                        var result = await HttpContentJsonExtensions.ReadFromJsonAsync(response.Content, JsonContext.Default.AntiCaptchaTaskResultFull, cancellationToken: token).ConfigureAwait(false);

                        if (result is null) {
                            return true;
                        }

                        if (result.Status == "ready" || result.ErrorId != 0) {
                            outerResult = result;
                            return false;
                        }

                        return true;
                    }),
                    options.PendingCount,
                    options.StartTimeoutSeconds * 1000);

            return outerResult;
        }
    }
}
*/