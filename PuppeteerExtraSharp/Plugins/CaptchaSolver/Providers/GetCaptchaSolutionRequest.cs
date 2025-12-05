using PuppeteerExtraSharp.Plugins.CaptchaSolver.Enums;
namespace PuppeteerExtraSharp.Plugins.CaptchaSolver.Providers;

public class GetCaptchaSolutionRequest
{
    public string SiteKey { get; set; }
    public string PageUrl { get; set; }
    public CaptchaVersion Version { get; set; }
    public CaptchaVendor Vendor { get; set; }
    public string DataS;
    public string Action { get; set; }
    public bool? IsEnterprise { get; set; }
    public bool IsInvisible { get; set; }
    public double MinScore { get; set; }
    public string? Gt { get; set; }
    public string? Challenge { get; set; }
    public string? CaptchaId { get; set; }
}
