using PuppeteerExtraSharpLite.Plugins.Stealth;

using PuppeteerSharp;

namespace PuppeteerExtraSharpLite.Tests.StealthPluginTests;

public partial class StealthPluginTests {
    [Fact]
    public async Task Stealth_Plugin_PlugStandardEvasions_ShouldNot_BeDetected() {
        var pluginManager = new PluginManager();
        pluginManager.Register(new StealthPlugin()).Register(StealthPlugin.GetStandardEvasions());

        await using var browser = await pluginManager.LaunchAsync();
        using var page = await browser.NewPageAsync();

        await page.GoToAsync("https://google.com");

        var webdriver = await page.EvaluateExpressionAsync<bool>("navigator.webdriver");
        Assert.False(webdriver);

        var headlessUserAgent = await page.EvaluateExpressionAsync<string>("window.navigator.userAgent");
        Assert.DoesNotContain("Headless", headlessUserAgent);

        var webDriverOverridden =
            await page.EvaluateExpressionAsync<bool>(
                "Object.getOwnPropertyDescriptor(navigator.__proto__, 'webdriver') !== undefined");
        Assert.True(webDriverOverridden);

        var plugins = await page.EvaluateExpressionAsync<int>("navigator.plugins.length");
        Assert.NotEqual(0, plugins);
    }

    [Fact]
    public async Task Stealth_Plugin_LaunchTest() {
        var pluginManager = new PluginManager();
        pluginManager.Register(new StealthPlugin());

        await using var browser = await pluginManager.LaunchAsync();
        using var page = await browser.NewPageAsync();

        await page.GoToAsync("https://bot.sannysoft.com");
        await page.ScreenshotAsync("Stealth.png", new ScreenshotOptions() {
            FullPage = true,
            Type = ScreenshotType.Png,
        });
    }
}