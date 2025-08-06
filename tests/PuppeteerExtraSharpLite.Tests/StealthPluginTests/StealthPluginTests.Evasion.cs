using PuppeteerExtraSharpLite.Tests.Utils;

using PuppeteerExtraSharpLite.Plugins.ExtraStealth.Evasions;
using PuppeteerExtraSharpLite.Plugins.ExtraStealth;

namespace PuppeteerExtraSharpLite.Tests.StealthPluginTests;

public partial class StealthPluginTests {
    [Fact]
    public async Task Evasion_Plugin_ShouldNot_HaveModifications() {
        var pluginManager = new PluginManager();
        pluginManager.Register(new StealthPlugin()).Register(new PluginEvasion());

        await using var browser = await pluginManager.LaunchAsync();
        using var page = await browser.NewPageAsync();

        await page.GoToAsync("https://google.com");

        var fingerPrint = await page.GetFingerPrint();

        var text = fingerPrint.GetRawText(); // for debug

        Assert.Equal(5, fingerPrint.GetProperty("plugins").GetArrayLength());
        Assert.Equal(2, fingerPrint.GetProperty("mimeTypes").GetArrayLength());
    }

    [Fact]
    public async Task Evasion_Plugin_HasMimeTypes() {
        var pluginManager = new PluginManager();
        pluginManager.Register(new StealthPlugin()).Register(new PluginEvasion());

        await using var browser = await pluginManager.LaunchAsync();
        using var page = await browser.NewPageAsync();

        await page.GoToAsync("https://google.com");

        var finger = await page.GetFingerPrint();

        Assert.Equal(5, finger.GetProperty("plugins").GetArrayLength());
        Assert.Equal(2, finger.GetProperty("mimeTypes").GetArrayLength());
    }
}
