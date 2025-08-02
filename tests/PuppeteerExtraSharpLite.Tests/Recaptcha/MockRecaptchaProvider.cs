using PuppeteerExtraSharpLite.Plugins.Recaptcha.Provider;

namespace PuppeteerExtraSharpLite.Tests.Recaptcha;

public class MockRecaptchaProvider : IRecaptchaProvider {
    public Task<string> GetSolution(string key, string pageUrl, string proxyStr = null) {
        return Task.FromResult("MOCK_CAPTCHA_TOKEN");
    }
}
