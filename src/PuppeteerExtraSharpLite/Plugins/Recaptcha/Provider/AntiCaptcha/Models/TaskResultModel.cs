using System.Text.Json.Serialization;

namespace PuppeteerExtraSharpLite.Plugins.Recaptcha.Provider.AntiCaptcha.Models;

public class TaskResultModel {
    [JsonPropertyName("errorId")]
    public int ErrorId { get; set; }

    [JsonPropertyName("status")]
    public string Status { get; set; } = string.Empty;

    [JsonPropertyName("solution")]
    public Solution? Solution { get; set; }

    [JsonPropertyName("cost")]
    public string Cost { get; set; } = string.Empty;

    [JsonPropertyName("ip")]
    public string Ip { get; set; } = string.Empty;

    [JsonPropertyName("createTime")]
    public int CreateTime { get; set; }

    [JsonPropertyName("endTime")]
    public int EndTime { get; set; }

    [JsonPropertyName("solveCount")]
    public int SolveCount { get; set; }
}

public class Solution {
    [JsonPropertyName("gRecaptchaResponse")]
    public string GRecaptchaResponse { get; set; } = string.Empty;

    [JsonPropertyName("cookies")]
    public Cookies? Cookies { get; set; }
}

public class Cookies {
    [JsonPropertyName("empty")]
    public string Empty { get; set; } = string.Empty;
}
