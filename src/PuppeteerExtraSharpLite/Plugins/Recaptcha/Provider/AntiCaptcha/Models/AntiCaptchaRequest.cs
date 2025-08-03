namespace PuppeteerExtraSharpLite.Plugins.Recaptcha.Provider.AntiCaptcha.Models;

public class AntiCaptchaRequest
{
    public string clientKey { get; set; } = string.Empty;
    public AntiCaptchaTask? task { get; set; }
}

public class RequestForResultTask
{
    public string clientKey { get; set; } = string.Empty;
    public int taskId { get; set; }
}

public struct AntiCaptchaTaskResult
{
    public int errorId { get; set; }
    public int taskId { get; set; }
}


public class AntiCaptchaTask
{
    public string type { get; set; } = string.Empty;
    public string websiteURL { get; set; } = string.Empty;
    public string websiteKey { get; set; } = string.Empty;
}
