using PuppeteerExtraSharpLite.Tests.Utils;

using PuppeteerExtraSharpLite.Plugins.ExtraStealth.Evasions;

namespace PuppeteerExtraSharpLite.Tests.StealthPluginTests.EvasionsTests;

public class PluginTest : BrowserDefault {
    [Fact]
    public async Task HasMimeTypes() {
        var plugin = new PluginEvasion();
        var page = await LaunchAndGetPage(plugin);
        await page.GoToAsync("https://google.com");

        var finger = await FingerPrint.GetFingerPrint(page);

        Assert.Equal(3, finger.GetProperty("plugins").GetPropertyCount());
        Assert.Equal(4, finger.GetProperty("mimeTypes").GetPropertyCount());
    }
}