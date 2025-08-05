using PuppeteerExtraSharpLite.Tests.Utils;

using PuppeteerExtraSharpLite.Plugins.ExtraStealth.Evasions;
using System.Text.Json;

namespace PuppeteerExtraSharpLite.Tests.StealthPluginTests.EvasionsTests;

public class LanguagesTest {
    [Fact]
    public async Task ShouldWork() {
        var pluginManager = new PluginManager();
        pluginManager.Register(new Languages());

        using var browser = await pluginManager.LaunchAsync();
        using var page = await browser.NewPageAsync();

        await page.GoToAsync("https://google.com");

        var fingerPrint = await FingerPrint.GetFingerPrint(page);

        var text = fingerPrint.GetRawText(); // for debug

        var languagesJson = fingerPrint.GetProperty("languages").GetRawText();

        var languages = JsonSerializer.Deserialize<string[]>(languagesJson);

        Assert.NotNull(languages);

        Assert.Contains("en-US", languages);
    }


    [Fact]
    public async Task ShouldWorkWithCustomSettings() {
        var pluginManager = new PluginManager();
        pluginManager.Register(new Languages(new StealthLanguagesOptions("fr-FR")));

        using var browser = await pluginManager.LaunchAsync();
        using var page = await browser.NewPageAsync();

        await page.GoToAsync("https://google.com");

        var fingerPrint = await FingerPrint.GetFingerPrint(page);

        var text = fingerPrint.GetRawText(); // for debug

        var languagesJson = fingerPrint.GetProperty("languages").GetRawText();

        var languages = JsonSerializer.Deserialize<string[]>(languagesJson);

        Assert.NotNull(languages);

        Assert.Contains("en-US", languages);
    }
}