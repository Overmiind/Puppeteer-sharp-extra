using System.Threading.Tasks;

namespace PuppeteerExtraSharp.Plugins.Recaptcha.Provider;

public interface IRecaptchaProvider
{
    public Task<string> GetSolutionAsync(GetRecaptchaSolutionRequest request);
}