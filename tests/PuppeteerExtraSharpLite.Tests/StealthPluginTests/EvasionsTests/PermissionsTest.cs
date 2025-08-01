using PuppeteerExtraSharpLite.Tests.Utils;

using PuppeteerExtraSharpLite.Plugins.ExtraStealth.Evasions;

namespace PuppeteerExtraSharpLite.Tests.StealthPluginTests.EvasionsTests;

public class PermissionsTest : BrowserDefault {
    [Fact]
    public async Task ShouldBeDeniedInHttpSite() {
        var plugin = new Permissions();
        var page = await LaunchAndGetPage(plugin);
        await page.GoToAsync("http://info.cern.ch/");

        var finger = await FingerPrint.GetFingerPrint(page);

        Assert.Equal("denied", finger.GetProperty("permissions").GetProperty("state").GetString());
        Assert.Equal("denied", finger.GetProperty("permissions").GetProperty("permission").GetString());
    }
}
