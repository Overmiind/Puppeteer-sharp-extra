using PuppeteerExtraSharpLite.Plugins.Stealth;

namespace PuppeteerExtraSharpLite.Tests.StealthPluginTests;

public partial class StealthPluginTests {
    [Fact]
    public async Task Permissions_Plugin_ShouldBe_DeniedInHttpSite() {
        var pluginManager = new PluginManager();
        pluginManager.Register(new StealthPlugin()).Register(new PermissionsPlugin());

        await using var browser = await pluginManager.LaunchAsync();
        var context = await browser.CreateBrowserContextAsync();
        await using var page = await context.NewPageAsync();

        await page.GoToAsync("http://info.cern.ch/");

        var finger = await page.GetFingerPrint();
        var s = finger.ToString(); // for debug

        Assert.Equal("prompt", finger.GetProperty("permissions").GetProperty("state").GetString());
        Assert.Equal("default", finger.GetProperty("permissions").GetProperty("permission").GetString());
    }
}
