namespace PuppeteerExtraSharp.Plugins.Recaptcha.Models;

public class FilteredCaptcha
{
    public Captcha Captcha { get; set; }
    public string FilteredReason { get; set; }
}