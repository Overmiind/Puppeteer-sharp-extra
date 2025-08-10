using System.Text.Json.Serialization;

namespace PuppeteerSharpToolkit.Plugins.Recaptcha.Provider.AntiCaptcha;

/// <summary>
/// Result model returned by Anti-Captcha for task status queries.
/// </summary>
public class AntiCaptchaTaskResultModel {
    /// <summary>
    /// Error identifier, 0 means success.
    /// </summary>
    [JsonPropertyName("errorId")]
    public int ErrorId { get; set; }

    /// <summary>
    /// Task state, e.g., <c>processing</c> or <c>ready</c>.
    /// </summary>
    [JsonPropertyName("status")]
    public string Status { get; set; } = string.Empty;

    /// <summary>
    /// Solution payload when the task is ready.
    /// </summary>
    [JsonPropertyName("solution")]
    public AntiCaptchaSolution? Solution { get; set; }

    /// <summary>
    /// Reported cost of the task.
    /// </summary>
    [JsonPropertyName("cost")]
    public string Cost { get; set; } = string.Empty;

    /// <summary>
    /// IP address used for solving (if provided).
    /// </summary>
    [JsonPropertyName("ip")]
    public string Ip { get; set; } = string.Empty;

    /// <summary>
    /// Unix timestamp when the task was created.
    /// </summary>
    [JsonPropertyName("createTime")]
    public int CreateTime { get; set; }

    /// <summary>
    /// Unix timestamp when the task finished.
    /// </summary>
    [JsonPropertyName("endTime")]
    public int EndTime { get; set; }

    /// <summary>
    /// Number of solutions attempted.
    /// </summary>
    [JsonPropertyName("solveCount")]
    public int SolveCount { get; set; }
}

/// <summary>
/// Anti-Captcha solution wrapper containing the token and optional cookies.
/// </summary>
public class AntiCaptchaSolution {
    /// <summary>
    /// reCAPTCHA solution token string.
    /// </summary>
    [JsonPropertyName("gRecaptchaResponse")]
    public string GRecaptchaResponse { get; set; } = string.Empty;

    /// <summary>
    /// Optional cookies returned with the solution.
    /// </summary>
    [JsonPropertyName("cookies")]
    public AntiCaptchaCookies? Cookies { get; set; }
}

/// <summary>
/// Placeholder for cookies block in Anti-Captcha results.
/// </summary>
public class AntiCaptchaCookies {
    /// <summary>
    /// Placeholder property; API currently returns an empty object.
    /// </summary>
    [JsonPropertyName("empty")]
    public string Empty { get; set; } = string.Empty;
}
