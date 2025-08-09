using PuppeteerExtraSharpLite.Plugins.Stealth;

namespace PuppeteerExtraSharpLite.Tests.StealthPluginTests;

public partial class StealthPluginTests {
    [Fact]
    public async Task UserAgent_Plugin_Test() {
        var pluginManager = new PluginManager();
        pluginManager.Register(new UserAgentPlugin());

        await using var browser = await pluginManager.LaunchAsync();
        await using var page = await browser.NewPageAsync();

        await page.GoToAsync("https://google.com");

        var finger = await page.GetFingerPrint();
        Assert.DoesNotContain("HeadlessChrome", finger.GetProperty("userAgent").GetString());
    }
}