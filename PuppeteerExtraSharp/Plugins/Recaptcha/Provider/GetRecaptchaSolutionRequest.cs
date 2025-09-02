namespace PuppeteerExtraSharp.Plugins.Recaptcha.Provider;

public class GetRecaptchaSolutionRequest
{
    public string SiteKey { get; set; }
    public string PageUrl { get; set; }
    public CaptchaVersion Version { get; set; }
    
    public string DataS;
    public string Action { get; set; }
    public bool? IsEnterprise { get; set; }
    public bool IsInvisible { get; set; }
    public double MinV3RecaptchaScore { get; set; }
}

public enum CaptchaVersion
{
    V2,
    V3,
    // TODO: not supported yet
    HCaptcha
}