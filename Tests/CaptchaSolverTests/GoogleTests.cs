using System.Threading.Tasks;
using PuppeteerExtraSharp.Plugins.CaptchaSolver;
using PuppeteerExtraSharp.Plugins.CaptchaSolver.Interfaces;
using PuppeteerExtraSharp.Plugins.CaptchaSolver.Models;
using PuppeteerSharp;
using Xunit;

namespace Extra.Tests.CaptchaSolverTests;

public class GoogleTests : CaptchaSolverTestsBase
{
    [Theory]
    [MemberData(nameof(Providers))]
    public async Task ShouldSolveCheckbox(ICaptchaSolverProvider provider)
    {
        var plugin = new CaptchaSolverPlugin(provider);
        var page = await LaunchAndGetPageAsync(plugin);

        await page.GoToAsync("https://recaptcha-demo.appspot.com/recaptcha-v2-checkbox.php");

        var result = await plugin.SolveCaptchaAsync(page, new CaptchaOptions
        {
            SolveInViewportOnly = true,
            SolveScoreBased = false,
        });

        Assert.Null(result.Error);
        Assert.NotEmpty(result.Solved);
        Assert.NotEmpty(result.Filtered);
        Assert.All(result.Filtered, captcha => Assert.True(captcha.Captcha.IsInvisible));

        await page.ClickAsync("button.form-field[type='submit']");
        await Task.Delay(1000);
        var answerElement =
            await page.EvaluateExpressionAsync<string>(
                "document.querySelector(\"body > main > h2:nth-child(3)\").textContent");

        Assert.Equal("Success!", answerElement);
    }

    [Theory]
    [MemberData(nameof(Providers))]
    public async Task ShouldSolveInvisible(ICaptchaSolverProvider provider)
    {
        var plugin = new CaptchaSolverPlugin(provider);
        var page = await LaunchAndGetPageAsync(plugin);

        await page.GoToAsync("https://recaptcha-demo.appspot.com/recaptcha-v2-invisible.php", new NavigationOptions()
        {
            WaitUntil =
            [
                WaitUntilNavigation.Networkidle0
            ]
        });

        var result = await plugin.SolveCaptchaAsync(page, new CaptchaOptions
        {
            SolveInvisibleChallenges = true,
            SolveScoreBased = false,
        });

        Assert.Null(result.Error);
        Assert.NotEmpty(result.Filtered);
        Assert.NotEmpty(result.Solved);
        Assert.All(result.Filtered, captcha => Assert.Equal(CaptchaType.score, captcha.Captcha.CaptchaType));

        await Task.Delay(1000);
        var answerElement =
            await page.EvaluateExpressionAsync<string>(
                "document.querySelector(\"body > main > h2:nth-child(3)\").textContent");

        Assert.Equal("Success!", answerElement);
    }

    [Theory]
    [MemberData(nameof(Providers))]
    public async Task ShouldtSolveInvisible(ICaptchaSolverProvider provider)
    {
        var plugin = new CaptchaSolverPlugin(provider);
        var page = await LaunchAndGetPageAsync(plugin);

        await page.GoToAsync("https://recaptcha-demo.appspot.com/recaptcha-v2-invisible.php");

        var result = await plugin.SolveCaptchaAsync(page, new CaptchaOptions
        {
            SolveInvisibleChallenges = false,
            SolveScoreBased = false,
        });

        Assert.Empty(result.Solved);
        Assert.NotEmpty(result.Filtered);
    }

    [Theory]
    [MemberData(nameof(Providers))]
    public async Task ShouldSolveV3Captcha(ICaptchaSolverProvider provider)
    {
        var plugin = new CaptchaSolverPlugin(provider);
        var page = await LaunchAndGetPageAsync(plugin);

        await page.GoToAsync("https://recaptcha-demo.appspot.com/recaptcha-v3-request-scores.php");

        var result = await plugin.SolveCaptchaAsync(page);

        Assert.NotEmpty(result.Solved);
        Assert.Empty(result.Filtered);
    }

    [Theory]
    [MemberData(nameof(Providers))]
    public async Task ShouldntSolveWhenNoCaptcha(ICaptchaSolverProvider provider)
    {
        var plugin = new CaptchaSolverPlugin(provider);
        var page = await LaunchAndGetPageAsync(plugin);

        await page.GoToAsync("https://google.com");

        var result = await plugin.SolveCaptchaAsync(page);

        Assert.Empty(result.Solved);
        Assert.Empty(result.Filtered);
    }
}