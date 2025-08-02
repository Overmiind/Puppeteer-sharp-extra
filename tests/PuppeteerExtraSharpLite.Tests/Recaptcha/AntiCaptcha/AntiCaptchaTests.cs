using PuppeteerExtraSharpLite.Tests.Utils;

using PuppeteerSharp;
using PuppeteerExtraSharpLite.Plugins.Recaptcha;

namespace PuppeteerExtraSharpLite.Tests.Recaptcha.AntiCaptcha;

[Collection("Captcha")]
public class AntiCaptchaTests : RecaptchaTestBase {
    [Fact]
    public async Task ShouldThrowCaptchaExceptionWhenCaptchaNotFound() {
        Assert.SkipUnless(Helper.TryGetEnvironmentVariable("AntiCaptchaKey", out var antiCaptchaKey),
            "AntiCaptchaKey environment variable is not set. Skipping test.");

        var plugin = new RecaptchaPlugin(new Plugins.Recaptcha.Provider.AntiCaptcha.AntiCaptcha(antiCaptchaKey));

        await using var browser = await LaunchWithPluginAsync(plugin);

        var page = await browser.NewPageAsync();
        await page.GoToAsync("https://lessons.zennolab.com/ru/index");
        var result = await plugin.SolveCaptchaAsync(page);
        Assert.NotNull(result.Exception);
        Assert.False(result.IsSuccess);
    }

    [Fact]
    public async Task ShouldSolveCaptchaWithSubmitButton() {
        Assert.SkipUnless(Helper.TryGetEnvironmentVariable("AntiCaptchaKey", out var antiCaptchaKey),
            "AntiCaptchaKey environment variable is not set. Skipping test.");

        var plugin = new RecaptchaPlugin(new Plugins.Recaptcha.Provider.AntiCaptcha.AntiCaptcha(antiCaptchaKey));

        await using var browser = await LaunchWithPluginAsync(plugin);

        var page = await browser.NewPageAsync();
        await page.GoToAsync("https://lessons.zennolab.com/captchas/recaptcha/v2_simple.php?level=low");
        var result = await plugin.SolveCaptchaAsync(page);

        Assert.Null(result.Exception);

        var button = await page.QuerySelectorAsync("input[type='submit']");
        await button.ClickAsync();

        // Wait for the success verification element to appear
        await page.WaitForSelectorAsync("div[id='main'] div[class='description'] h2", new WaitForSelectorOptions { Timeout = 10000 });
        await CheckSuccessVerify(page);
    }

    [Fact]
    public async Task ShouldSolveCaptchaWithCallback() {
        Assert.SkipUnless(Helper.TryGetEnvironmentVariable("AntiCaptchaKey", out var antiCaptchaKey),
            "AntiCaptchaKey environment variable is not set. Skipping test.");

        var plugin = new RecaptchaPlugin(new Plugins.Recaptcha.Provider.AntiCaptcha.AntiCaptcha(antiCaptchaKey));

        await using var browser = await LaunchWithPluginAsync(plugin);
        var page = await browser.NewPageAsync();
        await page.GoToAsync("https://lessons.zennolab.com/captchas/recaptcha/v2_nosubmit.php?level=low");
        var result = await plugin.SolveCaptchaAsync(page);

        Assert.Null(result.Exception);

        // Wait for the success verification element to appear
        await page.WaitForSelectorAsync("div[id='main'] div[class='description'] h2", new WaitForSelectorOptions { Timeout = 10000 });
        await CheckSuccessVerify(page);
    }

    private async Task CheckSuccessVerify(IPage page) {
        var successElement = await page.QuerySelectorAsync("div[id='main'] div[class='description'] h2");
        var elementValue = await (await successElement.GetPropertyAsync("textContent")).JsonValueAsync<string>();
        Assert.NotNull(successElement);
        Assert.Equal("Успешная верификация!", elementValue);
    }
}