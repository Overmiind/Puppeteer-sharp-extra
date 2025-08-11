using System.Text.Json.Serialization;

namespace PuppeteerSharpToolkit.Plugins.Recaptcha.TwoCaptcha;

/// <summary>
/// 2Captcha API response wrapper for both task creation and result queries.
/// </summary>
public class TwoCaptchaResponse
{
    /// <summary>
    /// Status code: 1 indicates success, otherwise an error.
    /// </summary>
    [JsonPropertyName("status")]
    public int Status { get; set; }

    /// <summary>
    /// Message content; on success contains either the task id (create) or the token (result).
    /// </summary>
    [JsonPropertyName("request")]
    public string Request { get; set; } = string.Empty;
}
