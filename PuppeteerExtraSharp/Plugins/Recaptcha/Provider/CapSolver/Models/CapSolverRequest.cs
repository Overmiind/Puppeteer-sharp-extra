namespace PuppeteerExtraSharp.Plugins.Recaptcha.Provider.CapSolver.Models;

public class CapSolverRequest
{
    public string clientKey { get; set; }
    public string taskId { get; set; }
    public CapSolverTaskRequest task { get; set; }
    
}

public class CapSolverTaskRequest
{
    public string type { get; set; }
    public string websiteURL { get; set; }
    public string websiteKey { get; set; }
}