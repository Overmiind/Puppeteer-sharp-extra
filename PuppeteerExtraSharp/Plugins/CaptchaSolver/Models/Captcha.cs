using PuppeteerExtraSharp.Plugins.CaptchaSolver.Enums;

namespace PuppeteerExtraSharp.Plugins.CaptchaSolver.Models;

public class Captcha
{
    public string Sitekey { get; set; }
    public string Callback { get; set; }
    public CaptchaVendor Vendor { get; set; }
    public string Id { get; set; }
    public string S { get; set; }
    public int WidgetId { get; set; }
    public CaptchaDisplay Display { get; set; }
    public string Action { get; set; }
    public string Url { get; set; }
    public bool HasResponseElement { get; set; }
    public bool IsEnterprise { get; set; }
    public bool IsInViewport { get; set; }
    public bool IsInvisible { get; set; }
    public bool HasActiveChallengePopup { get; set; }
    public bool HasChallengeFrame { get; set; }

    public string? Gt { get; set; }
    public string? Challenge { get; set; }
    public string? CaptchaId { get; set; }
    public string? Version { get; set; }

    public CaptchaType CaptchaType { get; set; }

    public Captcha()
    {
        Display = new CaptchaDisplay();
    }
}
