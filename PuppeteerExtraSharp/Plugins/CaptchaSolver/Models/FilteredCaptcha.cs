namespace PuppeteerExtraSharp.Plugins.CaptchaSolver.Models;

public class FilteredCaptcha
{
    public Captcha Captcha { get; set; }
    public string FilteredReason { get; set; }
}