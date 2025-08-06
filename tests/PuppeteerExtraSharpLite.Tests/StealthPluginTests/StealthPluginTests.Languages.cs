using System.Text.Json;
using PuppeteerExtraSharpLite.Plugins.Stealth;

namespace PuppeteerExtraSharpLite.Tests.StealthPluginTests;

public partial class StealthPluginTests {
    [Fact]
    public async Task Languages_Plugin_Test() {
        var pluginManager = new PluginManager();
        pluginManager.Register(new StealthPlugin()).Register(new LanguagesPlugin());

        await using var browser = await pluginManager.LaunchAsync();
        using var page = await browser.NewPageAsync();

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
        pluginManager.Register(new StealthPlugin()).Register(new LanguagesPlugin("fr-FR"));

        await using var browser = await pluginManager.LaunchAsync();
        using var page = await browser.NewPageAsync();

        await page.GoToAsync("https://google.com");

        var fingerPrint = await page.GetFingerPrint();

        var text = fingerPrint.GetRawText(); // for debug

        var languagesJson = fingerPrint.GetProperty("languages").GetRawText();

        var languages = JsonSerializer.Deserialize<string[]>(languagesJson);

        Assert.NotNull(languages);

        Assert.Contains("en-US", languages);
    }
}