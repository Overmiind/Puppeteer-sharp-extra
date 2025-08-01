namespace PuppeteerExtraSharpLite.Plugins.Recaptcha.Provider._2Captcha.Models;

internal class TwoCaptchaRequest
{
    public string key { get; set; }
}

internal class TwoCaptchaTask: TwoCaptchaRequest
{
    public string method { get; set; } = "userrecaptcha";
    public string googlekey { get; set; }
    public string pageurl { get; set; }
}

internal class TwoCaptchaRequestForResult: TwoCaptchaRequest
{
    public string action { get; set; } = "get";
    public string id { get; set; }
}
