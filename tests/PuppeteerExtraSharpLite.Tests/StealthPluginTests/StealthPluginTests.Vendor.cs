using PuppeteerSharpToolkit.Plugins;

namespace PuppeteerSharpToolkit.Tests.StealthPluginTests;

public partial class StealthPluginTests {
    [Fact]
    public async Task Vendor_Plugin_Test() {
        var pluginManager = new PluginManager();
        pluginManager.Register(new VendorPlugin());

        await using var browser = await pluginManager.LaunchAsync();
        var context = await browser.CreateBrowserContextAsync();
        await using var page = await context.NewPageAsync();

        await page.GoToAsync("https://google.com");

        var vendor = await page.EvaluateExpressionAsync<string>("navigator.vendor");
        Assert.Equal("Google Inc.", vendor);
    }
}
