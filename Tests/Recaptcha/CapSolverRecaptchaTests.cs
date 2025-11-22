using System;
using System.Linq;
using System.Threading.Tasks;
using Extra.Tests.Properties;
using PuppeteerExtraSharp.Plugins.Recaptcha;
using PuppeteerExtraSharp.Plugins.Recaptcha.Models;
using PuppeteerExtraSharp.Plugins.Recaptcha.Provider;
using PuppeteerExtraSharp.Plugins.Recaptcha.Provider.CapSolver;
using PuppeteerExtraSharp.Plugins.Recaptcha.Provider.TwoCaptcha;
using Xunit;

namespace Extra.Tests.Recaptcha;

public class CapSolverRecaptchaTests : BrowserDefault
{
    [Fact]
    public async Task ShouldSolveInvisible()
    {
        var plugin = new RecaptchaPlugin(new CapSolver(Resources.CapSolverKey, new CaptchaProviderOptions()
        {
            StartTimeout = TimeSpan.FromSeconds(10), MaxPollingAttempts = 30, ApiTimeout = TimeSpan.FromMinutes(3),
        }));
        var page = await LaunchAndGetPageAsync(plugin);

        await page.GoToAsync("https://recaptcha-demo.appspot.com/recaptcha-v2-invisible.php");

        var result = await plugin.SolveCaptchaAsync(page, new RecaptchaSolveOptions());

        Assert.Null(result.Error);
        Assert.NotEmpty(result.Solved);

        await Task.Delay(3000);
        var answerElement =
            await page.EvaluateExpressionAsync<string>(
                "document.querySelector(\"body > main > h2:nth-child(3)\").textContent");

        Assert.Equal("Success!", answerElement);
    }

    [Fact]
    public async Task ShouldSolveCheckbox()
    {
        var plugin = new RecaptchaPlugin(new CapSolver(Resources.CapSolverKey, new CaptchaProviderOptions()
        {
            StartTimeout = TimeSpan.FromSeconds(10), MaxPollingAttempts = 30, ApiTimeout = TimeSpan.FromMinutes(3),
        }));
        var page = await LaunchAndGetPageAsync(plugin);

        await page.GoToAsync("https://recaptcha-demo.appspot.com/recaptcha-v2-checkbox.php");

        var result = await plugin.SolveCaptchaAsync(page, new RecaptchaSolveOptions
        {
            SolveInViewportOnly = true,
            SolveScoreBased = false,
        });

        Assert.Null(result.Error);
        Assert.NotEmpty(result.Solved);
        Assert.NotEmpty(result.Filtered);
        Assert.All(result.Filtered, captcha => Assert.True(captcha.Captcha.IsInvisible));

        await page.ClickAsync("button.form-field[type='submit']");
        await Task.Delay(2000);
        var answerElement =
            await page.EvaluateExpressionAsync<string>(
                "document.querySelector(\"body > main > h2:nth-child(3)\").textContent");

        Assert.Equal("Success!", answerElement);
    }

    [Fact]
    public async Task ShouldtSolveInvisible()
    {
        var plugin = new RecaptchaPlugin(new CapSolver(Resources.CapSolverKey, new CaptchaProviderOptions()
        {
            StartTimeout = TimeSpan.FromSeconds(10), MaxPollingAttempts = 30, ApiTimeout = TimeSpan.FromMinutes(3),
        }));
        var page = await LaunchAndGetPageAsync(plugin);

        await page.GoToAsync("https://recaptcha-demo.appspot.com/recaptcha-v2-invisible.php");

        var result = await plugin.SolveCaptchaAsync(page, new RecaptchaSolveOptions
        {
            SolveInvisibleChallenges = false,
            SolveScoreBased = false,
        });

        Assert.Empty(result.Solved);
        Assert.NotEmpty(result.Filtered);
    }

    [Fact]
    public async Task ShouldSolveV3Captcha()
    {
        var plugin = new RecaptchaPlugin(new CapSolver(Resources.CapSolverKey, new CaptchaProviderOptions()
        {
            StartTimeout = TimeSpan.FromSeconds(10), MaxPollingAttempts = 30, ApiTimeout = TimeSpan.FromMinutes(3),
        }));
        var page = await LaunchAndGetPageAsync(plugin);

        await page.GoToAsync("https://recaptcha-demo.appspot.com/recaptcha-v3-request-scores.php");

        var result = await plugin.SolveCaptchaAsync(page);

        Assert.NotEmpty(result.Solved);
        Assert.Empty(result.Filtered);
    }

    [Fact]
    public async Task ShouldntSolveWhenNoCaptcha()
    {
        var plugin = new RecaptchaPlugin(new CapSolver(Resources.CapSolverKey));
        var page = await LaunchAndGetPageAsync(plugin);

        await page.GoToAsync("https://google.com");

        var result = await plugin.SolveCaptchaAsync(page);

        Assert.Empty(result.Solved);
        Assert.Empty(result.Filtered);
    }
}