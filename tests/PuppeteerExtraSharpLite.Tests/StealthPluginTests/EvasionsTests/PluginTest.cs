using PuppeteerExtraSharpLite.Tests.Utils;

using PuppeteerExtraSharpLite.Plugins.ExtraStealth.Evasions;

namespace PuppeteerExtraSharpLite.Tests.StealthPluginTests.EvasionsTests;

public class PluginTest : BrowserDefault {
    [Fact]
    public async Task HasMimeTypes() {
        var plugin = new PluginEvasion();
        var page = await LaunchAndGetPage(plugin);
        await page.GoToAsync("https://google.com");

        var finger = await new FingerPrint().GetFingerPrint(page);

        Assert.Equal(3, finger["plugins"].Count());
        Assert.Equal(4, finger["mimeTypes"].Count());
    }
}