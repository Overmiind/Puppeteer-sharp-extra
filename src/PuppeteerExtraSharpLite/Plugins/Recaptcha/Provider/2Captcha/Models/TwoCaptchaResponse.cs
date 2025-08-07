using System.Text.Json.Serialization;

namespace PuppeteerExtraSharpLite.Plugins.Recaptcha.Provider._2Captcha.Models;

public class TwoCaptchaResponse
{
    [JsonPropertyName("status")]
    public int Status { get; set; }

    [JsonPropertyName("request")]
    public string Request { get; set; } = string.Empty;
}
