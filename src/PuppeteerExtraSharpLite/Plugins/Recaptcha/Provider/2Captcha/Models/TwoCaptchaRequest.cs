using System.Text.Json.Serialization;

namespace PuppeteerExtraSharpLite.Plugins.Recaptcha.Provider._2Captcha.Models;

public class TwoCaptchaRequest {
    [JsonPropertyName("key")]
    public string Key { get; set; } = string.Empty;
}

public class TwoCaptchaTask : TwoCaptchaRequest {
    [JsonPropertyName("method")]
    public string Method { get; set; } = "userrecaptcha";

    [JsonPropertyName("googlekey")]
    public string GoogleKey { get; set; } = string.Empty;

    [JsonPropertyName("pageurl")]
    public string PageUrl { get; set; } = string.Empty;
}

public class TwoCaptchaRequestForResult : TwoCaptchaRequest {
    [JsonPropertyName("action")]
    public string Action { get; set; } = "get";

    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;
}
