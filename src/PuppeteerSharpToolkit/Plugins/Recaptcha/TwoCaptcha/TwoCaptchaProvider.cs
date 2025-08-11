using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace PuppeteerSharpToolkit.Plugins.Recaptcha.TwoCaptcha;

/// <summary>
/// 2Captcha (rucaptcha) provider implementation for solving Google reCAPTCHA.
/// </summary>
public class TwoCaptchaProvider : IRecaptchaProvider {
    private readonly HttpClient _client;
    private readonly string _userKey;
    private readonly ProviderOptions _options;

    /// <summary>
    /// Initializes the provider with default polling options.
    /// </summary>
    /// <param name="client">HTTP client used for API calls.</param>
    /// <param name="userKey">2Captcha API key.</param>
    public TwoCaptchaProvider(HttpClient client, string userKey) : this(client, userKey, ProviderOptions.Default) { }

    /// <summary>
    /// Initializes the provider with custom options.
    /// </summary>
    /// <param name="client">HTTP client used for API calls.</param>
    /// <param name="userKey">2Captcha API key.</param>
    /// <param name="options">Polling options.</param>
    public TwoCaptchaProvider(HttpClient client, string userKey, ProviderOptions options) {
        _client = client;
        _userKey = userKey;
        _options = options;
    }

    /// <summary>
    /// Creates a task and polls 2Captcha until the token is ready.
    /// </summary>
    /// <param name="pageUrl">URL hosting the widget.</param>
    /// <param name="proxyStr">Optional proxy string.</param>
    /// <param name="token">Cancellation token.</param>
    /// <returns>Solution token.</returns>
    /// <exception cref="HttpRequestException">Thrown when API reports an error.</exception>
    public async Task<string> GetSolutionAsync(string pageUrl, string proxyStr = "", CancellationToken token = default) {
        var task = await Api.CreateTaskAsync(_client, _userKey, pageUrl, token);

        ThrowErrorIfBadStatus(task);

        await Task.Delay(_options.StartTimeoutSeconds * 1000, token);

        var result = await Api.GetSolution(_client, _userKey, task.Request, _options, token);

        ThrowErrorIfBadStatus(result);

        return result.Request;
    }

    /// <summary>
    /// Throws when the 2Captcha response indicates an error or missing data.
    /// </summary>
    private static void ThrowErrorIfBadStatus(TwoCaptchaResponse response) {
        if (response.Status != 1 || string.IsNullOrEmpty(response.Request)) {
            throw new HttpRequestException($"Two captcha request ends with error [{response.Status}] {response.Request}");
        }
    }

    /// <summary>
    /// Internal 2Captcha API helpers.
    /// </summary>
    public static class Api {
        private static readonly Uri Host = new("https://rucaptcha.com");

        /// <summary>
        /// Creates a 2Captcha task for a site key and page.
        /// </summary>
        /// <param name="client">HTTP client.</param>
        /// <param name="userKey">2Captcha API key.</param>
        /// <param name="pageUrl">Target page URL.</param>
        /// <param name="token">Cancellation token.</param>
        /// <returns>Response containing the created task identifier.</returns>
        public static async Task<TwoCaptchaResponse> CreateTaskAsync(HttpClient client, string userKey, string pageUrl, CancellationToken token = default) {
            Uri uri = new(Host, "in.php");
            Dictionary<string, string> parameters = new() {
                ["key"] = userKey,
                ["pageurl"] = pageUrl,
                ["json"] = "1",
                ["method"] = "userrecaptcha"
            };
            string url = parameters.AddAsQueryTo(uri.AbsoluteUri);

            using var message = new HttpRequestMessage(HttpMethod.Post, url);
            message.Headers.Add("Accept", "application/json");

            using var response = await client.SendAsync(message, token).ConfigureAwait(false);

            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync(JsonContext.Default.TwoCaptchaResponse, cancellationToken: token)
            .ConfigureAwait(false)
            ?? throw new JsonException($"Failed to deserialize content into {nameof(TwoCaptchaResponse)}");
        }


        /// <summary>
        /// Polls 2Captcha for the solution token using the task id.
        /// </summary>
        /// <param name="client">HTTP client.</param>
        /// <param name="userKey">2Captcha API key.</param>
        /// <param name="id">Task id returned by <see cref="CreateTaskAsync"/>.</param>
        /// <param name="options">Polling options.</param>
        /// <param name="token">Cancellation token.</param>
        /// <returns>Response containing the final solution token.</returns>
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

                    var result = await response.Content.ReadFromJsonAsync(JsonContext.Default.TwoCaptchaResponse, cancellationToken: token).ConfigureAwait(false);

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

            return outerResult ?? new TwoCaptchaResponse();
        }
    }
}
