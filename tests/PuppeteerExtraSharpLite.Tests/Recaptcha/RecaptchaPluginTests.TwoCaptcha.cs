using PuppeteerExtraSharpLite.Plugins.Recaptcha;
using PuppeteerExtraSharpLite.Plugins.Recaptcha.Provider.TwoCaptcha;

namespace PuppeteerExtraSharpLite.Tests.Recaptcha;

public partial class RecaptchaPluginTests {
    [Fact]
    public async Task TwoCaptcha_Plugin_Should_ResolveCaptchaInGooglePage() {
        Assert.SkipWhen(_twoCaptchaKey.Length == 0, TwoCaptchaReason);

        using var client = new HttpClient();
        var provider = new TwoCaptchaProvider(client, _twoCaptchaKey);
        var plugin = new RecaptchaPlugin(provider);

        var pluginManager = new PluginManager();
        pluginManager.Register(plugin);

        await using var browser = await pluginManager.LaunchAsync();
        using var page = await browser.NewPageAsync();

        await page.GoToAsync("https://www.google.com/recaptcha/api2/demo");

        await plugin.SolveCaptchaAsync(page);
        var button = await page.QuerySelectorAsync("input[id='recaptcha-demo-submit']");
        await button.ClickAsync();
        await page.WaitForNavigationAsync();
        var successElement = await page.QuerySelectorAsync("div[class='recaptcha-success']");

        Assert.NotNull(successElement);
    }

    [Fact]
    public async Task TwoCaptcha_Plugin_Should_SolveInvisibleCaptcha() {
        Assert.SkipWhen(_twoCaptchaKey.Length == 0, TwoCaptchaReason);

        using var client = new HttpClient();
        var provider = new TwoCaptchaProvider(client, _twoCaptchaKey);
        var plugin = new RecaptchaPlugin(provider);

        var pluginManager = new PluginManager();
        pluginManager.Register(plugin);

        await using var browser = await pluginManager.LaunchAsync();
        using var page = await browser.NewPageAsync();

        await page.GoToAsync("https://recaptcha-demo.appspot.com/recaptcha-v2-invisible.php");

        var result = await plugin.SolveCaptchaAsync(page);

        Assert.Null(result.Exception);
        await page.WaitForNavigationAsync();
        var elements = await page.QuerySelectorAllAsync("main h2");

        Assert.Equal(2, elements.Length);

        var elementProperty = await (await elements[1].GetPropertyAsync("textContent")).JsonValueAsync<string>();
        Assert.Equal("Success!", elementProperty);
    }
}