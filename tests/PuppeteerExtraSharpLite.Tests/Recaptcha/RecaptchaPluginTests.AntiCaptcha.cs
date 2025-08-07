using PuppeteerExtraSharpLite.Plugins.Recaptcha;
using PuppeteerExtraSharpLite.Plugins.Recaptcha.Provider.AntiCaptcha;

using PuppeteerSharp;

namespace PuppeteerExtraSharpLite.Tests.Recaptcha;

public partial class RecaptchaPluginTests {
    [Fact]
    public async Task AntiCaptcha_Plugin_Should_ThrowCaptchaException_When_CaptchaNotFound() {
        Assert.SkipWhen(_antiCaptchaKey.Length == 0, AntiCaptchaReason);

        using var client = new HttpClient();
        var provider = new AntiCaptchaProvider(client, _antiCaptchaKey);
        var plugin = new RecaptchaPlugin(provider);

        var pluginManager = new PluginManager();
        pluginManager.Register(plugin);

        await using var browser = await pluginManager.LaunchAsync();
        using var page = await browser.NewPageAsync();

        await page.GoToAsync("https://lessons.zennolab.com/ru/index");
        var result = await plugin.SolveCaptchaAsync(page);
        Assert.NotNull(result.Exception);
        Assert.False(result.IsSuccess);
    }

    [Fact]
    public async Task AntiCaptcha_Plugin_Should_SolveCaptchaWithSubmitButton() {
        Assert.SkipWhen(_antiCaptchaKey.Length == 0, AntiCaptchaReason);

        using var client = new HttpClient();
        var provider = new AntiCaptchaProvider(client, _antiCaptchaKey);
        var plugin = new RecaptchaPlugin(provider);

        var pluginManager = new PluginManager();
        pluginManager.Register(plugin);

        await using var browser = await pluginManager.LaunchAsync();
        using var page = await browser.NewPageAsync();

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
    public async Task AntiCaptcha_Plugin_ShouldSolve_CaptchaWithCallback() {
        Assert.SkipWhen(_antiCaptchaKey.Length == 0, AntiCaptchaReason);

        using var client = new HttpClient();
        var provider = new AntiCaptchaProvider(client, _antiCaptchaKey);
        var plugin = new RecaptchaPlugin(provider);

        var pluginManager = new PluginManager();
        pluginManager.Register(plugin);

        await using var browser = await pluginManager.LaunchAsync();
        using var page = await browser.NewPageAsync();

        await page.GoToAsync("https://lessons.zennolab.com/captchas/recaptcha/v2_nosubmit.php?level=low");
        var result = await plugin.SolveCaptchaAsync(page);

        Assert.Null(result.Exception);

        // Wait for the success verification element to appear
        await page.WaitForSelectorAsync("div[id='main'] div[class='description'] h2", new WaitForSelectorOptions { Timeout = 10000 });
        await CheckSuccessVerify(page);
    }

    private static async Task CheckSuccessVerify(IPage page) {
        var successElement = await page.QuerySelectorAsync("div[id='main'] div[class='description'] h2");
        var elementValue = await (await successElement.GetPropertyAsync("textContent")).JsonValueAsync<string>();
        Assert.NotNull(successElement);
        Assert.Equal("Успешная верификация!", elementValue);
    }
}