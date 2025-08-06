using PuppeteerExtraSharpLite.Tests.Utils;

using PuppeteerExtraSharpLite.Plugins.ExtraStealth.Evasions;
using PuppeteerExtraSharpLite.Plugins.ExtraStealth;

namespace PuppeteerExtraSharpLite.Tests.StealthPluginTests;

public partial class StealthPluginTests {
    [Fact]
    public async Task Permissions_Plugin_ShouldBe_DeniedInHttpSite() {
        var pluginManager = new PluginManager();
        pluginManager.Register(new StealthPlugin()).Register(new Permissions());

        await using var browser = await pluginManager.LaunchAsync();
        using var page = await browser.NewPageAsync();

        await page.GoToAsync("http://info.cern.ch/");

        var finger = await page.GetFingerPrint();
        var s = finger.ToString(); // for debug

        Assert.Equal("prompt", finger.GetProperty("permissions").GetProperty("state").GetString());
        Assert.Equal("default", finger.GetProperty("permissions").GetProperty("permission").GetString());
    }
}
