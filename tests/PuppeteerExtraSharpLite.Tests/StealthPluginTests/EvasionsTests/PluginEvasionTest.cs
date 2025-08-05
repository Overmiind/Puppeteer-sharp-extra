using PuppeteerExtraSharpLite.Tests.Utils;

using PuppeteerExtraSharpLite.Plugins.ExtraStealth.Evasions;
using PuppeteerExtraSharpLite.Plugins.ExtraStealth;

namespace PuppeteerExtraSharpLite.Tests.StealthPluginTests.EvasionsTests;

public class PluginEvasionTest {
    [Fact]
    public async Task ShouldNotHaveModifications() {
        var pluginManager = new PluginManager();
        pluginManager.Register(new StealthPlugin()).Register(new PluginEvasion());

        await using var browser = await pluginManager.LaunchAsync();
        using var page = await browser.NewPageAsync();

        await page.GoToAsync("https://google.com");

        var fingerPrint = await FingerPrint.GetFingerPrint(page);

        var text = fingerPrint.GetRawText(); // for debug

        Assert.Equal(5, fingerPrint.GetProperty("plugins").GetArrayLength());
        Assert.Equal(2, fingerPrint.GetProperty("mimeTypes").GetArrayLength());
    }
}
