using PuppeteerSharpToolkit.Plugins;

namespace PuppeteerSharpToolkit.Tests.StealthPluginTests;

public partial class StealthPluginTests {
    [Fact]
    public async Task Evasion_Plugin_ShouldNot_HaveModifications() {
        var pluginManager = new PluginManager();
        pluginManager.Register(new EvasionPlugin());

        await using var browser = await pluginManager.LaunchAsync();
        var context = await browser.CreateBrowserContextAsync();
        await using var page = await context.NewPageAsync();

        await page.GoToAsync("https://google.com");

        var fingerPrint = await page.GetFingerPrint();

        var text = fingerPrint.GetRawText(); // for debug

        Assert.Equal(5, fingerPrint.GetProperty("plugins").GetArrayLength());
        Assert.Equal(2, fingerPrint.GetProperty("mimeTypes").GetArrayLength());
    }

    [Fact]
    public async Task Evasion_Plugin_HasMimeTypes() {
        var pluginManager = new PluginManager();
        pluginManager.Register(new EvasionPlugin());

        await using var browser = await pluginManager.LaunchAsync();
        var context = await browser.CreateBrowserContextAsync();
        await using var page = await context.NewPageAsync();

        await page.GoToAsync("https://google.com");

        var finger = await page.GetFingerPrint();

        Assert.Equal(5, finger.GetProperty("plugins").GetArrayLength());
        Assert.Equal(2, finger.GetProperty("mimeTypes").GetArrayLength());
    }
}
