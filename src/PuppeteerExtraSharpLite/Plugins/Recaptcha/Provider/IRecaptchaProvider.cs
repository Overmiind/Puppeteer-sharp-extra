namespace PuppeteerExtraSharpLite.Plugins.Recaptcha.Provider;

public interface IRecaptchaProvider {
    Task<string> GetSolution(string key, string pageUrl, string proxyStr = "");
}
