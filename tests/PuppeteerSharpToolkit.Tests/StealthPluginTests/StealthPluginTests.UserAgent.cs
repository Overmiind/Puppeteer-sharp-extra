using PuppeteerSharpToolkit.Plugins;

namespace PuppeteerSharpToolkit.Tests.StealthPluginTests;

public partial class StealthPluginTests {
    [Fact]
    public async Task UserAgent_Plugin_Test() {
        var pluginManager = new PluginManager();
        pluginManager.Register(new UserAgentPlugin());

        await using var browser = await pluginManager.LaunchAsync();
        var context = await browser.CreateBrowserContextAsync();
        await using var page = await context.NewPageAsync();

        await page.GoToAsync("https://google.com");

        var finger = await page.GetFingerPrint();
        Assert.DoesNotContain("HeadlessChrome", finger.GetProperty("userAgent").GetString());
    }
}
