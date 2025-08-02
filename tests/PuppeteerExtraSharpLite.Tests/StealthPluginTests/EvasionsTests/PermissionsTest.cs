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
        var s = finger.ToString(); // for debug

        var permissionsState = finger.GetProperty("permissions").GetProperty("state").GetString();
        var permissionsPermission = finger.GetProperty("permissions").GetProperty("permission").GetString();

        //TODO: Ensure denied is really the expected
        Assert.Equal("denied", permissionsState);
        Assert.Equal("denied", permissionsPermission);
    }
}
