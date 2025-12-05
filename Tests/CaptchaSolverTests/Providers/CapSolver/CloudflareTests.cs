using System;
using System.Threading.Tasks;
using Extra.Tests.Properties;
using PuppeteerExtraSharp.Plugins.CaptchaSolver;
using PuppeteerExtraSharp.Plugins.CaptchaSolver.Models;
using PuppeteerExtraSharp.Plugins.CaptchaSolver.Providers;
using Provider = PuppeteerExtraSharp.Plugins.CaptchaSolver.Providers.CapSolver;
using Xunit;
namespace Extra.Tests.CaptchaSolverTests.Providers.CapSolver;

public class CloudflareTests : BrowserDefault
{
    [Fact]
    public async Task ShouldSolveCheckbox()
    {
        var plugin = new CaptchaSolverPlugin(new Provider.CapSolver(Resources.CapSolverKey, new CaptchaProviderOptions()
        {
            StartTimeout = TimeSpan.FromSeconds(10),
            MaxPollingAttempts = 30,
            ApiTimeout = TimeSpan.FromMinutes(3),
        }));
        var page = await LaunchAndGetPageAsync(plugin);

        await page.GoToAsync("https://clifford.io/demo/cloudflare-turnstile");

        var result = await plugin.SolveCaptchaAsync(page, new CaptchaSolverOptions
        {
            SolveInViewportOnly = true,
            SolveScoreBased = false,
        });

        Assert.Null(result.Error);
        Assert.NotEmpty(result.Solved);
        Assert.Empty(result.Filtered);

        await page.ClickAsync("button[type='submit']");
        await Task.Delay(2000);
        var answerElement = await page.EvaluateExpressionAsync<string>("document.querySelector(\"body\").textContent");

        Assert.Contains("Passed Cloudflare Turnstile check", answerElement);
    }
}
