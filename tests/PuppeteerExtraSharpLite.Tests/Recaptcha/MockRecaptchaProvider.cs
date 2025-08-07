using PuppeteerExtraSharpLite.Plugins.Recaptcha.Provider;

namespace PuppeteerExtraSharpLite.Tests.Recaptcha;

public class MockRecaptchaProvider : IRecaptchaProvider {
    public Task<string> GetSolutionAsync(string key, string pageUrl, string? proxyStr = null, CancellationToken token = default) {
        return Task.FromResult("MOCK_CAPTCHA_TOKEN");
    }
}