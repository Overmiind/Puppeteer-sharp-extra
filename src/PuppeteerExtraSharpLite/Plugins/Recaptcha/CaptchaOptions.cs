namespace PuppeteerExtraSharpLite.Plugins.Recaptcha;

public record CaptchaOptions(bool VisualFeedBack, bool IsThrowException) {
    public static readonly CaptchaOptions Default = new(false, false);
}