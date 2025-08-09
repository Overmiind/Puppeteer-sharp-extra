using System.Text.Json.Serialization;

namespace PuppeteerExtraSharpLite.Plugins.Recaptcha.Provider.TwoCaptcha;

/// <summary>
/// Base request carrying the 2Captcha API key.
/// </summary>
public class TwoCaptchaRequest {
    /// <summary>
    /// 2Captcha API key.
    /// </summary>
    [JsonPropertyName("key")]
    public string Key { get; set; } = string.Empty;
}

/// <summary>
/// Task creation request for solving reCAPTCHA via 2Captcha.
/// </summary>
public class TwoCaptchaTask : TwoCaptchaRequest {
    /// <summary>
    /// API method name. Defaults to <c>userrecaptcha</c>.
    /// </summary>
    [JsonPropertyName("method")]
    public string Method { get; set; } = "userrecaptcha";

    /// <summary>
    /// reCAPTCHA site key from the target page.
    /// </summary>
    [JsonPropertyName("googlekey")]
    public string GoogleKey { get; set; } = string.Empty;

    /// <summary>
    /// URL of the page containing the widget.
    /// </summary>
    [JsonPropertyName("pageurl")]
    public string PageUrl { get; set; } = string.Empty;
}

/// <summary>
/// Request to retrieve the solution for a previously created 2Captcha task.
/// </summary>
public class TwoCaptchaRequestForResult : TwoCaptchaRequest {
    /// <summary>
    /// Action type; must be <c>get</c>.
    /// </summary>
    [JsonPropertyName("action")]
    public string Action { get; set; } = "get";

    /// <summary>
    /// Identifier returned from the task creation call.
    /// </summary>
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;
}