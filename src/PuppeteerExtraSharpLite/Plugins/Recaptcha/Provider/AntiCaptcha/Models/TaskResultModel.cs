namespace PuppeteerExtraSharpLite.Plugins.Recaptcha.Provider.AntiCaptcha.Models;

public class TaskResultModel {
    public int errorId { get; set; }
    public string status { get; set; } = string.Empty;
    public Solution? solution { get; set; }
    public string cost { get; set; } = string.Empty;
    public string ip { get; set; } = string.Empty;
    public int createTime { get; set; }
    public int endTime { get; set; }
    public int solveCount { get; set; }
}

public class Solution {
    public string gRecaptchaResponse { get; set; } = string.Empty;
    public Cookies? cookies { get; set; }
}

public class Cookies {
    public string empty { get; set; } = string.Empty;
}
