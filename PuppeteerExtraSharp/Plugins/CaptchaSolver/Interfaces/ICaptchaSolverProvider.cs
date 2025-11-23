using System.Threading.Tasks;
using PuppeteerExtraSharp.Plugins.CaptchaSolver.Providers;
namespace PuppeteerExtraSharp.Plugins.CaptchaSolver.Interfaces;

public interface ICaptchaSolverProvider
{
    public Task<string> GetSolutionAsync(GetCaptchaSolutionRequest request);
}
