namespace PuppeteerExtraSharpLite.Plugins.Recaptcha;

/// <summary>
/// Options controlling how Recaptcha solving behaves in the UI and error handling.
/// </summary>
/// <param name="VisualFeedBack">Enables visual feedback when filling the Recaptcha response.</param>
/// <param name="IsThrowException">If true, exceptions are thrown instead of returning failure results.</param>
public record CaptchaOptions(bool VisualFeedBack, bool IsThrowException) {
    /// <summary>
    /// Default options: no visual feedback and do not throw exceptions.
    /// </summary>
    public static readonly CaptchaOptions Default = new(false, false);
}