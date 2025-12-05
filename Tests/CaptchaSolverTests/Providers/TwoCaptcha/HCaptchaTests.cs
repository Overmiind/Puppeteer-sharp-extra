using System;
using System.Threading.Tasks;
using Extra.Tests.Properties;
using PuppeteerExtraSharp.Plugins.CaptchaSolver;
using PuppeteerExtraSharp.Plugins.CaptchaSolver.Models;
using PuppeteerExtraSharp.Plugins.CaptchaSolver.Providers;
using Xunit;
using Provider = PuppeteerExtraSharp.Plugins.CaptchaSolver.Providers.TwoCaptcha;
namespace Extra.Tests.CaptchaSolverTests.Providers.TwoCaptcha;

public class HCaptchaTests : BrowserDefault
{
    [Fact]
    public async Task ShouldSolveCheckbox()
    {
        var plugin = new CaptchaSolverPlugin(new Provider.TwoCaptcha(Resources.TwoCaptchaKey, new CaptchaProviderOptions()
        {
            StartTimeout = TimeSpan.FromSeconds(10),
            MaxPollingAttempts = 30,
            ApiTimeout = TimeSpan.FromMinutes(3),
        }));
        var page = await LaunchAndGetPageAsync(plugin);

        await page.GoToAsync("https://nopecha.com/demo/hcaptcha");

        var result = await plugin.FindCaptchaAsync(page, new CaptchaSolverOptions
        {
            SolveInViewportOnly = true,
            SolveScoreBased = false,
        });

        Assert.Null(result.Error);
        Assert.NotEmpty(result.Captchas);

        /*var result = await plugin.SolveCaptchaAsync(page, new CaptchaSolverOptions
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
        var answerElement = await page.EvaluateExpressionAsync<string>("document.querySelector(\"#token_3\").textContent");
        Assert.Equal("success", answerElement);*/
    }
}
