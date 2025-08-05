using PuppeteerExtraSharpLite.Tests.Utils;

using PuppeteerExtraSharpLite.Plugins.ExtraStealth.Evasions;

namespace PuppeteerExtraSharpLite.Tests.StealthPluginTests.EvasionsTests;

public class PluginTest {
    [Fact]
    public async Task HasMimeTypes() {
        var pluginManager = new PluginManager();
        pluginManager.Register(new PluginEvasion());

        await using var browser = await pluginManager.LaunchAsync();
        using var page = await browser.NewPageAsync();

        await page.GoToAsync("https://google.com");

        var finger = await FingerPrint.GetFingerPrint(page);

        Assert.Equal(5, finger.GetProperty("plugins").GetArrayLength());
        Assert.Equal(2, finger.GetProperty("mimeTypes").GetArrayLength());
    }
}