using System;
using System.Threading.Tasks;

namespace PuppeteerExtraSharp.Plugins.Recaptcha.Provider.TwoCaptcha;

public class TwoCaptcha : IRecaptchaProvider
{
    private readonly CaptchaProviderOptions _options;
    private readonly TwoCaptchaApi _api;

    public TwoCaptcha(string key, CaptchaProviderOptions options = null)
    {
        _options = options ?? new CaptchaProviderOptions();
        _api = new TwoCaptchaApi(key, _options);
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