using System.Text.Json.Serialization;

namespace PuppeteerSharpToolkit.Plugins.Recaptcha.Provider.AntiCaptcha;

/// <summary>
/// Request payload for Anti-Captcha createTask API.
/// </summary>
public class AntiCaptchaRequest {
    /// <summary>
    /// Anti-Captcha account client key.
    /// </summary>
    [JsonPropertyName("clientKey")]
    public string ClientKey { get; set; } = string.Empty;

    /// <summary>
    /// Captcha solving task description.
    /// </summary>
    [JsonPropertyName("task")]
    public AntiCaptchaTask? Task { get; set; }
}

/// <summary>
/// Request payload for Anti-Captcha getTaskResult API.
/// </summary>
public class AntiCaptchaRequestForResultTask {
    /// <summary>
    /// Anti-Captcha account client key.
    /// </summary>
    [JsonPropertyName("clientKey")]
    public string ClientKey { get; set; } = string.Empty;

    /// <summary>
    /// Identifier of the previously created task.
    /// </summary>
    [JsonPropertyName("taskId")]
    public int TaskId { get; set; }
}

/// <summary>
/// Response returned after creating a task in Anti-Captcha.
/// </summary>
public class AntiCaptchaTaskResult {
    /// <summary>
    /// Error identifier, 0 means success.
    /// </summary>
    [JsonPropertyName("errorId")]
    public int ErrorId { get; set; }

    /// <summary>
    /// Identifier of the created task.
    /// </summary>
    [JsonPropertyName("taskId")]
    public int TaskId { get; set; }
}


/// <summary>
/// NoCaptcha task description for Anti-Captcha.
/// </summary>
public class AntiCaptchaTask {
    /// <summary>
    /// Task type, e.g., NoCaptchaTaskProxyless.
    /// </summary>
    [JsonPropertyName("type")]
    public string Type { get; set; } = string.Empty;

    /// <summary>
    /// Target page URL that hosts the widget.
    /// </summary>
    [JsonPropertyName("websiteURL")]
    public string WebsiteUrl { get; set; } = string.Empty;

    /// <summary>
    /// reCAPTCHA site key.
    /// </summary>
    [JsonPropertyName("websiteKey")]
    public string WebsiteKey { get; set; } = string.Empty;
}
