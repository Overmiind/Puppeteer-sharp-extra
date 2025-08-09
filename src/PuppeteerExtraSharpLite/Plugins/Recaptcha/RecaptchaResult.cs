namespace PuppeteerExtraSharpLite.Plugins.Recaptcha;

/// <summary>
/// Represents the result of a reCAPTCHA operation.
/// </summary>
public record RecaptchaResult {
    /// <summary>
    /// Indicates whether the operation completed successfully.
    /// </summary>
    public bool IsSuccess { get; init; } = true;

    /// <summary>
    /// On success, contains the extracted value (e.g., site key) or auxiliary info.
    /// On failure, may include the page URL or empty.
    /// </summary>
    public string Value { get; init; } = string.Empty;

    /// <summary>
    /// Optional error message describing the failure.
    /// </summary>
    public string Exception { get; init; } = string.Empty;
}