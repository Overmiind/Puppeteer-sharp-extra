using PuppeteerExtraSharpLite.Plugins.ExtraStealth;

namespace PuppeteerExtraSharpLite.Tests.StealthPluginTests;

public class StealthPluginTests {
    [Fact]
    public async Task ShouldBeNotDetected() {
        var pluginManager = new PluginManager();
        pluginManager.Register(StealthPlugin.GetStandardEvasions());
        pluginManager.Register(new StealthPlugin());

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
}