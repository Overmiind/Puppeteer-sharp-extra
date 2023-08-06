using System.Net.Http;
using System.Threading.Tasks;

namespace PuppeteerExtraSharp.Plugins.Recaptcha.Provider.CapSolver;

public class CapSolver : IRecaptchaProvider
{
    private readonly ProviderOptions _options;
    private readonly CapSolverApi _api;
        
    public CapSolver(string key, ProviderOptions options = null)
    {
        _options = options ?? ProviderOptions.CreateDefaultOptions();
        _api = new CapSolverApi(key, _options);
    }
    public async Task<string> GetSolution(string key, string pageUrl, string proxyStr = null)
    {
        var task = await _api.CreateTaskAsync(pageUrl, key);
        await System.Threading.Tasks.Task.Delay(_options.StartTimeoutSeconds * 1000);
        var result = await _api.PendingForResult(task.taskId);

        if (result.status != "ready" || result.solution is null || result.errorId != 0)
            throw new HttpRequestException($"CapSolver request ends with error - {result.errorId}");

        return result.solution.gRecaptchaResponse;
    }
}