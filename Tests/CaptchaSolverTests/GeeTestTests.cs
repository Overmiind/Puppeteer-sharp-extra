using System;
using System.Threading.Tasks;
using PuppeteerExtraSharp.Plugins.CaptchaSolver;
using PuppeteerExtraSharp.Plugins.CaptchaSolver.Enums;
using PuppeteerExtraSharp.Plugins.CaptchaSolver.Interfaces;
using PuppeteerExtraSharp.Plugins.CaptchaSolver.Models;
using Xunit;

namespace Extra.Tests.CaptchaSolverTests;

public class GeeTestTests : CaptchaSolverTestsBase
{
    [Theory]
    [MemberData(nameof(Providers))]
    public async Task ShouldSolveV3(ICaptchaSolverProvider provider)
    {
        var plugin = new CaptchaSolverPlugin(provider);
        var page = await LaunchAndGetPageAsync(plugin);

        await page.GoToAsync("https://2captcha.com/fr/demo/geetest");

        var result = await plugin.SolveCaptchaAsync(page, new CaptchaOptions
        {
            SolveInViewportOnly = true,
            SolveScoreBased = false,
            EnabledVendors = [CaptchaVendor.GeeTest],
            CaptchaWaitTimeout = TimeSpan.FromSeconds(20)
        });

        Assert.Null(result.Error);
        Assert.NotEmpty(result.Solved);
        Assert.Empty(result.Filtered);

        await page.ClickAsync("#geetest-demo-form button[type='submit']");
        await Task.Delay(2000);
        var answerElement = await page.EvaluateExpressionAsync<string>("document.querySelector(\"#geetest-demo-form code\")?.textContent");

        Assert.Contains("\"success\": true", answerElement);
    }

    [Theory]
    [MemberData(nameof(Providers))]
    public async Task ShouldSolveV4(ICaptchaSolverProvider provider)
    {
        var plugin = new CaptchaSolverPlugin(provider);
        var page = await LaunchAndGetPageAsync(plugin);

        await page.GoToAsync("https://2captcha.com/demo/geetest-v4");

        var result = await plugin.SolveCaptchaAsync(page, new CaptchaOptions
        {
            SolveInViewportOnly = true,
            SolveScoreBased = false,
            CaptchaWaitTimeout = TimeSpan.FromSeconds(20)
        });

        Assert.Null(result.Error);
        Assert.NotEmpty(result.Solved);
        Assert.Empty(result.Filtered);

        await page.ClickAsync("button[data-action=demo_action][type=submit]");
        await Task.Delay(2000);
        var answerElement = await page.EvaluateExpressionAsync<string>("document.querySelector(\"code\")?.textContent");

        Assert.Contains("\"result\": \"success\"", answerElement);
    }
}
