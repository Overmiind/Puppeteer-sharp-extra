using System.Text.Json;
namespace PuppeteerExtraSharp.Plugins.CaptchaSolver.Providers.CapSolver.Models;

internal class CapSolverCreateTaskResponse
{
    public int ErrorId { get; set; }
    public string TaskId { get; set; }
}

internal class CapSolverGetTaskResult
{
    public int ErrorId { get; set; }
    public string ErrorCode { get; set; }
    public string ErrorDescription { get; set; }
    public string Status { get; set; }
    public JsonElement Solution { get; set; }
    public string Cost { get; set; }
    public string Ip { get; set; }
    public long CreateTime { get; set; }
    public long EndTime { get; set; }
    public int SolveCount { get; set; }
}
