using PuppeteerExtraSharpLite.Tests.Utils;

using PuppeteerExtraSharpLite.Plugins.ExtraStealth.Evasions;

namespace PuppeteerExtraSharpLite.Tests.StealthPluginTests.EvasionsTests;

public class PermissionsTest {
    [Fact]
    public async Task ShouldBeDeniedInHttpSite() {
        var pluginManager = new PluginManager();
        pluginManager.Register(new Permissions());

        await using var browser = await pluginManager.LaunchAsync();
        using var page = await browser.NewPageAsync();

        await page.GoToAsync("http://info.cern.ch/");

        var finger = await FingerPrint.GetFingerPrint(page);
        var s = finger.ToString(); // for debug

        Assert.Equal("prompt", finger.GetProperty("permissions").GetProperty("state").GetString());
        Assert.Equal("default", finger.GetProperty("permissions").GetProperty("permission").GetString());
    }
}
