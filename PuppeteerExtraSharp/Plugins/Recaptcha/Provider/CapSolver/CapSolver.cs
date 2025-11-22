using System;
using System.Threading.Tasks;
namespace PuppeteerExtraSharp.Plugins.Recaptcha.Provider.CapSolver;

public class CapSolver : IRecaptchaProvider
{
    private readonly CaptchaProviderOptions _options;
    private readonly CapSolverApi _api;

    public CapSolver(string key, CaptchaProviderOptions options = null)
    {
        _options = options ?? new CaptchaProviderOptions();
        _api = new CapSolverApi(key, _options); 
    }

    public async Task<string> GetSolutionAsync(GetRecaptchaSolutionRequest request)
    {
        var task = await _api.CreateTaskAsync(request);

        await Task.Delay(_options.StartTimeout);

        var result = await _api.GetSolution(task.TaskId);

        if (result.Solution == null)
        {
            throw new ArgumentNullException(nameof(result.Solution), "Captcha solution can't be null");
        }

        return result.Solution.GRecaptchaResponse;
    }
}
