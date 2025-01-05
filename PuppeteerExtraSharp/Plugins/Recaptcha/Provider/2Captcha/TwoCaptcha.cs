using System.Net.Http;
using System.Threading.Tasks;
using PuppeteerExtraSharp.Plugins.Recaptcha.Provider._2Captcha.Models;

namespace PuppeteerExtraSharp.Plugins.Recaptcha.Provider._2Captcha;

public class TwoCaptcha : IRecaptchaProvider
{
    private readonly ProviderOptions _options;
    private readonly TwoCaptchaApi _api;

    public TwoCaptcha(string key, ProviderOptions options = null)
    {
        _options = options ?? ProviderOptions.CreateDefaultOptions();
        _api = new TwoCaptchaApi(key, _options);
    }

    public async Task<string> GetSolution(string key, string pageUrl, string proxyStr = null)
    {
        var task = await _api.CreateTaskAsync(key, pageUrl);

        ThrowErrorIfBadStatus(task);

        await Task.Delay(_options.StartTimeoutSeconds * 1000);

        var result = await _api.GetSolution(task.request);

        ThrowErrorIfBadStatus(result.Data);

        return result.Data.request;
    }

    private void ThrowErrorIfBadStatus(TwoCaptchaResponse response)
    {
        if (response.status != 1 || string.IsNullOrEmpty(response.request))
            throw new HttpRequestException(
                $"Two captcha request ends with error [{response.status}] {response.request}");
    }
}
