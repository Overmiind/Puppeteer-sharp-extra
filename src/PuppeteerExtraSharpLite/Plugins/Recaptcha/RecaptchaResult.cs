namespace PuppeteerExtraSharpLite.Plugins.Recaptcha;

/// <summary>
/// 
/// </summary>
/// <param name="IsSuccess"></param>
/// <param name="Exception"></param>
public record RecaptchaResult(bool IsSuccess = true, CaptchaException? Exception = default);

public class CaptchaException : Exception {
    public required string PageUrl { get; init; }
    public required string Content { get; init; }
}