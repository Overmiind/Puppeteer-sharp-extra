namespace PuppeteerExtraSharp.Plugins.Recaptcha.Provider.AntiCaptcha.Models;

public class AntiCaptchaRequest
{
    public string clientKey { get; set; }
    public AntiCaptchaTask task { get; set; }
}
