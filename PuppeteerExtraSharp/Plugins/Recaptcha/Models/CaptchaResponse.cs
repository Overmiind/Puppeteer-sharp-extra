using System.Collections.Generic;

namespace PuppeteerExtraSharp.Plugins.Recaptcha.Models;

public class CaptchaResponse
{
    public List<Captcha> Captchas { get; set; }
    public string Error { get; set; }
}