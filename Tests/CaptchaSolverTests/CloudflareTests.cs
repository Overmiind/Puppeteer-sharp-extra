using System.Threading.Tasks;
using PuppeteerExtraSharp.Plugins.CaptchaSolver;
using PuppeteerExtraSharp.Plugins.CaptchaSolver.Interfaces;
using PuppeteerExtraSharp.Plugins.CaptchaSolver.Models;
using Xunit;

namespace Extra.Tests.CaptchaSolverTests;

public class CloudflareTests : CaptchaSolverTestsBase
{
    [Theory]
    [MemberData(nameof(Providers))]
    public async Task ShouldSolveCheckbox(ICaptchaSolverProvider provider)
    {
        var plugin = new CaptchaSolverPlugin(provider);
        var page = await LaunchAndGetPageAsync(plugin);

        await page.GoToAsync("https://clifford.io/demo/cloudflare-turnstile");

        var result = await plugin.SolveCaptchaAsync(page, new CaptchaOptions
        {
            SolveInViewportOnly = true,
            SolveScoreBased = false,
        });

        Assert.Null(result.Error);
        Assert.NotEmpty(result.Solved);
        Assert.Empty(result.Filtered);

        await page.ClickAsync("button[type='submit']");
        await Task.Delay(2000);
        var answerElement =
            await page.EvaluateExpressionAsync<string>("document.querySelector(\"body\").textContent");

        Assert.Contains("Passed Cloudflare Turnstile check", answerElement);
    }
}