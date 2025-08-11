using PuppeteerSharpToolkit.Plugins.Recaptcha;
using PuppeteerSharpToolkit.Plugins.Recaptcha.TwoCaptcha;

namespace PuppeteerSharpToolkit.Tests.Recaptcha;

public class TwoCaptchaPluginTests {
    private readonly string _twoCaptchaKey = TestConfig.Config["TwoCaptchaKey"] ?? string.Empty;
	private const string TwoCaptchaReason = "TwoCaptchaKey user secret is not set";

    [Fact(Explicit = true)]
    public async Task TwoCaptcha_Plugin_Should_ResolveCaptchaInGooglePage() {
        Assert.SkipWhen(_twoCaptchaKey.Length == 0, TwoCaptchaReason);

        using var client = new HttpClient();
        var provider = new TwoCaptchaProvider(client, _twoCaptchaKey);
        var plugin = new RecaptchaPlugin(provider);

        var pluginManager = new PluginManager();
        pluginManager.Register(plugin);

        await using var browser = await pluginManager.LaunchAsync();
        var context = await browser.CreateBrowserContextAsync();
        await using var page = await context.NewPageAsync();

        await page.GoToAsync("https://www.google.com/recaptcha/api2/demo");

        await plugin.SolveCaptchaAsync(page, token: TestContext.Current.CancellationToken);
        var button = await page.QuerySelectorAsync("input[id='recaptcha-demo-submit']");
        await button.ClickAsync();
        await page.WaitForNavigationAsync();
        var successElement = await page.QuerySelectorAsync("div[class='recaptcha-success']");

        Assert.NotNull(successElement);
    }

    [Fact(Explicit = true)]
    public async Task TwoCaptcha_Plugin_Should_SolveInvisibleCaptcha() {
        Assert.SkipWhen(_twoCaptchaKey.Length == 0, TwoCaptchaReason);

        using var client = new HttpClient();
        var provider = new TwoCaptchaProvider(client, _twoCaptchaKey);
        var plugin = new RecaptchaPlugin(provider);

        var pluginManager = new PluginManager();
        pluginManager.Register(plugin);

        await using var browser = await pluginManager.LaunchAsync();
        var context = await browser.CreateBrowserContextAsync();
        await using var page = await context.NewPageAsync();

        await page.GoToAsync("https://recaptcha-demo.appspot.com/recaptcha-v2-invisible.php");

        var result = await plugin.SolveCaptchaAsync(page, token: TestContext.Current.CancellationToken);

        Assert.Null(result.Exception);
        await page.WaitForNavigationAsync();
        var elements = await page.QuerySelectorAllAsync("main h2");

        Assert.Equal(2, elements.Length);

        var elementProperty = await (await elements[1].GetPropertyAsync("textContent")).JsonValueAsync<string>();
        Assert.Equal("Success!", elementProperty);
    }
}
