namespace PuppeteerExtraSharpLite.Plugins.Recaptcha.Provider;

public interface IRecaptchaProvider {
    Task<string> GetSolutionAsync(string key, string pageUrl, string proxyStr = "", CancellationToken token = default);
}
