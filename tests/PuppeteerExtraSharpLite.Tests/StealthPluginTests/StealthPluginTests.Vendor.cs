using PuppeteerExtraSharpLite.Plugins.ExtraStealth;
using PuppeteerExtraSharpLite.Plugins.ExtraStealth.Evasions;

namespace PuppeteerExtraSharpLite.Tests.StealthPluginTests;

public partial class StealthPluginTests {
    [Fact]
    public async Task Vendor_Plugin_Test() {
        var pluginManager = new PluginManager();
        pluginManager.Register(new StealthPlugin()).Register(new Vendor());

        await using var browser = await pluginManager.LaunchAsync();
        using var page = await browser.NewPageAsync();

        await page.GoToAsync("https://google.com");

        var vendor = await page.EvaluateExpressionAsync<string>("navigator.vendor");
        Assert.Equal("Google Inc.", vendor);
    }
}
