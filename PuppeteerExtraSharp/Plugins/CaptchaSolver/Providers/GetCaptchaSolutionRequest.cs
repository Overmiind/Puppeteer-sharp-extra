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

    // GeeTest-specific
    public string? Gt { get; set; }
    public string? Challenge { get; set; }
    public string? CaptchaId { get; set; }

    // DataDome-specific
    public string? DataDomeCaptchaUrl { get; set; }
    public string? DataDomeCid { get; set; }
    public string? DataDomeHash { get; set; }
    public string? DataDomeUserAgent { get; set; }
    public string? DataDomeReferer { get; set; }

    // Proxy settings (required for DataDome)
    public string? ProxyType { get; set; }
    public string? ProxyAddress { get; set; }
    public int? ProxyPort { get; set; }
    public string? ProxyLogin { get; set; }
    public string? ProxyPassword { get; set; }
    public string? ProxySessionId { get; set; }
}
