using System.Threading.Tasks;
using PuppeteerExtraSharp.Plugins.CaptchaSolver;
using PuppeteerExtraSharp.Plugins.CaptchaSolver.Interfaces;
using PuppeteerExtraSharp.Plugins.CaptchaSolver.Models;
using Xunit;

namespace Extra.Tests.CaptchaSolverTests;

public class HCaptchaTests : CaptchaSolverTestsBase
{
    [Theory]
    [MemberData(nameof(Providers))]
    public async Task ShouldSolveCheckbox(ICaptchaSolverProvider provider)
    {
        var plugin = new CaptchaSolverPlugin(provider);
        var page = await LaunchAndGetPageAsync(plugin);

        await page.GoToAsync("https://nopecha.com/demo/hcaptcha");

        var result = await plugin.FindCaptchaAsync(page, new CaptchaOptions
        {
            SolveInViewportOnly = true,
            SolveScoreBased = false,
        });

        Assert.Null(result.Error);
        Assert.NotEmpty(result.Captchas);

        /* var result = await plugin.SolveCaptchaAsync(page, new CaptchaSolverOptions
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
