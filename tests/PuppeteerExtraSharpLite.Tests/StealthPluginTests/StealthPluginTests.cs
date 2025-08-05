using PuppeteerExtraSharpLite.Plugins.ExtraStealth;

namespace PuppeteerExtraSharpLite.Tests.StealthPluginTests;

public class StealthPluginTests : BrowserDefault {
    [Fact]
    public async Task ShouldBeNotDetected() {
        var pluginManager = new PluginManager();
        pluginManager.Register(StealthPlugin.GetStandardEvasions());
        pluginManager.Register(new StealthPlugin());

        using var browser = await pluginManager.LaunchAsync(CreateDefaultOptions());
        using var page = await browser.NewPageAsync();

        await page.GoToAsync("https://google.com");

        var webdriver = await page.EvaluateExpressionAsync<bool>("navigator.webdriver");
        Assert.False(webdriver);

        var headlessUserAgent = await page.EvaluateExpressionAsync<string>("window.navigator.userAgent");
        Assert.DoesNotContain("Headless", headlessUserAgent);

        var webDriverOverriden =
            await page.EvaluateExpressionAsync<bool>(
                "Object.getOwnPropertyDescriptor(navigator.__proto__, 'webdriver') !== undefined");
        Assert.True(webDriverOverriden);

        var plugins = await page.EvaluateExpressionAsync<int>("navigator.plugins.length");
        Assert.NotEqual(0, plugins);
    }
}