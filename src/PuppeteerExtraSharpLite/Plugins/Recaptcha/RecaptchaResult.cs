namespace PuppeteerExtraSharpLite.Plugins.Recaptcha;

public record RecaptchaResult {
    public bool IsSuccess { get; init; } = true;

    public string Value { get; init; } = string.Empty;

    public string Exception { get; init; } = string.Empty;
}