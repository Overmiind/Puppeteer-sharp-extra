using PuppeteerExtraSharpLite.Tests.Utils;

using PuppeteerExtraSharpLite.Plugins.ExtraStealth.Evasions;

namespace PuppeteerExtraSharpLite.Tests.StealthPluginTests.EvasionsTests;

public class PluginEvasionTest : BrowserDefault {
    [Fact]
    public async Task ShouldNotHaveModifications() {
        var stealthPlugin = new PluginEvasion();
        var page = await LaunchAndGetPage(stealthPlugin);

        await page.GoToAsync("https://google.com");


        var fingerPrint = await new FingerPrint().GetFingerPrint(page);

        Assert.Equal(3, fingerPrint["plugins"].Count());
        Assert.Equal(4, fingerPrint["mimeTypes"].Count());
    }
}
