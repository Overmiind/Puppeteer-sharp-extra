using System.Text.Json;
using PuppeteerExtraSharpLite.Plugins;

namespace PuppeteerExtraSharpLite.Tests.StealthPluginTests;

public partial class StealthPluginTests {
    [Fact]
    public async Task Languages_Plugin_Test() {
        var pluginManager = new PluginManager();
        pluginManager.Register(new LanguagesPlugin());

        await using var browser = await pluginManager.LaunchAsync();
        var context = await browser.CreateBrowserContextAsync();
        await using var page = await context.NewPageAsync();

        await page.GoToAsync("https://google.com");

        var fingerPrint = await page.GetFingerPrint();

        var text = fingerPrint.GetRawText(); // for debug

        var languagesJson = fingerPrint.GetProperty("languages").GetRawText();

        var languages = JsonSerializer.Deserialize<string[]>(languagesJson);

        Assert.NotNull(languages);

        Assert.Contains("en-US", languages);
    }


    [Fact]
    public async Task Languages_Plugin_Should_WorkWithCustomSettings() {
        var pluginManager = new PluginManager();
        pluginManager.Register(new LanguagesPlugin("fr-FR"));

        await using var browser = await pluginManager.LaunchAsync();
        var context = await browser.CreateBrowserContextAsync();
        await using var page = await context.NewPageAsync();

        await page.GoToAsync("https://google.com");

        var fingerPrint = await page.GetFingerPrint();

        var text = fingerPrint.GetRawText(); // for debug

        var languagesJson = fingerPrint.GetProperty("languages").GetRawText();

        var languages = JsonSerializer.Deserialize<string[]>(languagesJson);

        Assert.NotNull(languages);

        Assert.Contains("en-US", languages);
    }
}