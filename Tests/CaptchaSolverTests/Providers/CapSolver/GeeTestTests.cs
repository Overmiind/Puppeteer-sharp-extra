using System;
using System.Threading.Tasks;
using Extra.Tests.Properties;
using PuppeteerExtraSharp.Plugins.CaptchaSolver;
using PuppeteerExtraSharp.Plugins.CaptchaSolver.Models;
using PuppeteerExtraSharp.Plugins.CaptchaSolver.Providers;
using Xunit;
namespace Extra.Tests.CaptchaSolverTests.Providers.CapSolver;

public class GeeTestTests : BrowserDefault
{
    [Fact]
    public async Task ShouldSolveCheckbox()
    {
        var plugin = new CaptchaSolverPlugin(new PuppeteerExtraSharp.Plugins.CaptchaSolver.Providers.TwoCaptcha.TwoCaptcha(Resources.TwoCaptchaKey, new CaptchaProviderOptions()
        {
            StartTimeout = TimeSpan.FromSeconds(10),
            MaxPollingAttempts = 30,
            ApiTimeout = TimeSpan.FromMinutes(3),
        }));
        var page = await LaunchAndGetPageAsync(plugin);

        await page.GoToAsync("https://2captcha.com/fr/demo/geetest");

        var result = await plugin.SolveCaptchaAsync(page, new CaptchaSolverOptions
        {
            SolveInViewportOnly = true,
            SolveScoreBased = false,
        });

        Assert.Null(result.Error);
        Assert.NotEmpty(result.Solved);
        Assert.Empty(result.Filtered);

        await page.ClickAsync("#geetest-demo-form button[type='submit']");
        await Task.Delay(2000);
        var answerElement = await page.EvaluateExpressionAsync<string>("document.querySelector(\"#geetest-demo-form code\")?.textContent");

        Assert.Contains("\"success\": true", answerElement);
    } 
}
