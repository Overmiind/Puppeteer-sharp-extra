namespace PuppeteerExtraSharp.Plugins.Recaptcha.Provider.AntiCaptcha.Models;

public class TaskResultModel
{
    public int errorId { get; set; }
    public string status { get; set; }
    public Solution solution { get; set; }
    public string cost { get; set; }
    public string ip { get; set; }
    public int createTime { get; set; }
    public int endTime { get; set; }
    public string solveCount { get; set; }
}
