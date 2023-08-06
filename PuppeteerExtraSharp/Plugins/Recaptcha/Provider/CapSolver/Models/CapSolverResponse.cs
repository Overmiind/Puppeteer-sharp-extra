namespace PuppeteerExtraSharp.Plugins.Recaptcha.Provider.CapSolver.Models;

public class CapSolverTaskResponse
{
    public int errorId { get; set; }
    public string errorCode { get; set; }
    public string errorDescription { get; set; }
    public string taskId { get; set; }
    public string status { get; set; }
    public CapSolverTaskSolutionResponse solution { get; set; }
}

public class CapSolverTaskSolutionResponse
{
    public string userAgent { get; set; }
    public long expireTime { get; set; }
    public string gRecaptchaResponse { get; set; }
}