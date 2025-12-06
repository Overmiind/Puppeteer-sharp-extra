using PuppeteerExtraSharp.Plugins.CaptchaSolver.Enums;

namespace PuppeteerExtraSharp.Plugins.CaptchaSolver.Models;

public class FilteredCaptcha
{
    public CaptchaVendor Vendor { get; set; }
    public Captcha Captcha { get; set; }
    public string FilteredReason { get; set; }
}