using System;
using System.Text.Json;
using System.Threading.Tasks;
using PuppeteerExtraSharp.Plugins.CaptchaSolver.Interfaces;
namespace PuppeteerExtraSharp.Plugins.CaptchaSolver.Providers.CapSolver;

public class CapSolver : ICaptchaSolverProvider
{
    private readonly CaptchaProviderOptions _options;
    private readonly CapSolverApi _api;

    public CapSolver(string key, CaptchaProviderOptions options = null)
    {
        _options = options ?? new CaptchaProviderOptions();
        _api = new CapSolverApi(key, _options);
    }

    public async Task<string> GetSolutionAsync(GetCaptchaSolutionRequest request)
    {
        var task = await _api.CreateTaskAsync(request);

        await Task.Delay(_options.StartTimeout);

        var result = await _api.GetSolution(task.TaskId);

        if (result.Solution.ValueKind is JsonValueKind.Undefined)
        {
            throw new ArgumentNullException(nameof(result.Solution), "Captcha solution can't be null");
        }

        return result.Solution.ToString();
    }
}
