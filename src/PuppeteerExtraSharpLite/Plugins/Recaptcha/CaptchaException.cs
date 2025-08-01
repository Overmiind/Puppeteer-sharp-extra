using System;

namespace PuppeteerExtraSharpLite.Plugins.Recaptcha;

public class CaptchaException : Exception {
    public CaptchaException(string pageUrl, string content) {
        PageUrl = pageUrl;
        Content = content;
    }

    public string PageUrl { get; set; }
    public string Content { get; set; }
}