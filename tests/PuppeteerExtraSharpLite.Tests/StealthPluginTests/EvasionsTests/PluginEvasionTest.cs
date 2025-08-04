using PuppeteerExtraSharpLite.Tests.Utils;

using PuppeteerExtraSharpLite.Plugins.ExtraStealth.Evasions;

namespace PuppeteerExtraSharpLite.Tests.StealthPluginTests.EvasionsTests;

public class PluginEvasionTest : BrowserDefault {
    [Fact]
    public async Task ShouldNotHaveModifications() {
        var stealthPlugin = new PluginEvasion();
        var page = await LaunchAndGetPage(stealthPlugin);

        await page.GoToAsync("https://google.com");


        var fingerPrint = await FingerPrint.GetFingerPrint(page);

        var text = fingerPrint.GetRawText(); // for debug

        Assert.Equal(5, fingerPrint.GetProperty("plugins").GetArrayLength());
        Assert.Equal(2, fingerPrint.GetProperty("mimeTypes").GetArrayLength());
    }
}
