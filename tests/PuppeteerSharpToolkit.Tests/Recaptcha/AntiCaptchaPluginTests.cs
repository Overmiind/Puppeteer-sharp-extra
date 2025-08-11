/*

//TODO: Disabled pending "GET RECAPTCHA KEY" review

using PuppeteerSharpToolkit.Plugins.Recaptcha;
using PuppeteerSharpToolkit.Plugins.Recaptcha.AntiCaptcha;

using PuppeteerSharp;

namespace PuppeteerSharpToolkit.Tests.Recaptcha;

public class AntiCaptchaPluginTests {
    private readonly string _antiCaptchaKey = TestConfig.Config["AntiCaptchaKey"] ?? string.Empty;
	private const string AntiCaptchaReason = "AntiCaptchaKey user secret is not set";

    [Fact(Explicit = true)]
    public async Task AntiCaptcha_Plugin_Should_ThrowCaptchaException_When_CaptchaNotFound() {
        Assert.SkipWhen(_antiCaptchaKey.Length == 0, AntiCaptchaReason);

        using var client = new HttpClient();
        var provider = new AntiCaptchaProvider(client, _antiCaptchaKey);
        var plugin = new RecaptchaPlugin(provider);

        var pluginManager = new PluginManager();
        pluginManager.Register(plugin);

        await using var browser = await pluginManager.LaunchAsync();
        var context = await browser.CreateBrowserContextAsync();
        await using var page = await context.NewPageAsync();

        await page.GoToAsync("https://lessons.zennolab.com/ru/index");
        var result = await plugin.SolveCaptchaAsync(page, token: TestContext.Current.CancellationToken);
        Assert.NotNull(result.Exception);
        Assert.False(result.IsSuccess);
    }

    [Fact(Explicit = true)]
    public async Task AntiCaptcha_Plugin_Should_SolveCaptchaWithSubmitButton() {
        Assert.SkipWhen(_antiCaptchaKey.Length == 0, AntiCaptchaReason);

        using var client = new HttpClient();
        var provider = new AntiCaptchaProvider(client, _antiCaptchaKey);
        var plugin = new RecaptchaPlugin(provider);

        var pluginManager = new PluginManager();
        pluginManager.Register(plugin);

        await using var browser = await pluginManager.LaunchAsync();
        var context = await browser.CreateBrowserContextAsync();
        await using var page = await context.NewPageAsync();

        await page.GoToAsync("https://lessons.zennolab.com/captchas/recaptcha/v2_simple.php?level=low");
        var result = await plugin.SolveCaptchaAsync(page, token: TestContext.Current.CancellationToken);

        Assert.Null(result.Exception);

        var button = await page.QuerySelectorAsync("input[type='submit']");
        await button.ClickAsync();

        // Wait for the success verification element to appear
        await page.WaitForSelectorAsync("div[id='main'] div[class='description'] h2", new WaitForSelectorOptions { Timeout = 10000 });
        await CheckSuccessVerify(page);
    }

    [Fact(Explicit = true)]
    public async Task AntiCaptcha_Plugin_ShouldSolve_CaptchaWithCallback() {
        Assert.SkipWhen(_antiCaptchaKey.Length == 0, AntiCaptchaReason);

        using var client = new HttpClient();
        var provider = new AntiCaptchaProvider(client, _antiCaptchaKey);
        var plugin = new RecaptchaPlugin(provider);

        var pluginManager = new PluginManager();
        pluginManager.Register(plugin);

        await using var browser = await pluginManager.LaunchAsync();
        var context = await browser.CreateBrowserContextAsync();
        await using var page = await context.NewPageAsync();

        await page.GoToAsync("https://lessons.zennolab.com/captchas/recaptcha/v2_nosubmit.php?level=low");
        var result = await plugin.SolveCaptchaAsync(page, token: TestContext.Current.CancellationToken);

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
*/