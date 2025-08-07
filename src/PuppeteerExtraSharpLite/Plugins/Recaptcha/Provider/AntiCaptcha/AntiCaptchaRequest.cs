using System.Text.Json.Serialization;

namespace PuppeteerExtraSharpLite.Plugins.Recaptcha.Provider.AntiCaptcha;

public class AntiCaptchaRequest {
    [JsonPropertyName("clientKey")]
    public string ClientKey { get; set; } = string.Empty;

    [JsonPropertyName("task")]
    public AntiCaptchaTask? Task { get; set; }
}

public class AntiCaptchaRequestForResultTask {
    [JsonPropertyName("clientKey")]
    public string ClientKey { get; set; } = string.Empty;

    [JsonPropertyName("taskId")]
    public int TaskId { get; set; }
}

public class AntiCaptchaTaskResult {
    [JsonPropertyName("errorId")]
    public int ErrorId { get; set; }

    [JsonPropertyName("taskId")]
    public int TaskId { get; set; }
}


public class AntiCaptchaTask {
    [JsonPropertyName("type")]
    public string Type { get; set; } = string.Empty;

    [JsonPropertyName("websiteURL")]
    public string WebsiteURL { get; set; } = string.Empty;

    [JsonPropertyName("websiteKey")]
    public string WebsiteKey { get; set; } = string.Empty;
}
