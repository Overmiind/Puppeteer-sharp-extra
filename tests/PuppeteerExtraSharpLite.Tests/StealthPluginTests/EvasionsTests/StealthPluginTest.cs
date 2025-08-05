using PuppeteerExtraSharpLite.Plugins.ExtraStealth;

using PuppeteerSharp;

namespace PuppeteerExtraSharpLite.Tests.StealthPluginTests.EvasionsTests;

public class StealthPluginTest {
    [Fact]
    public async Task Test() {
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
