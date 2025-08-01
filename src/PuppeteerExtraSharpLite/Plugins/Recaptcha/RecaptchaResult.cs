namespace PuppeteerExtraSharpLite.Plugins.Recaptcha;

public class RecaptchaResult {
    public bool IsSuccess { get; set; } = true;
    public CaptchaException Exception { get; set; }
}