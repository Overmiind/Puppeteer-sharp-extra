using PuppeteerExtraSharpLite.Tests.Utils;

using PuppeteerExtraSharpLite.Plugins.ExtraStealth.Evasions;
using System.Text.Json;

namespace PuppeteerExtraSharpLite.Tests.StealthPluginTests.EvasionsTests;

public class LanguagesTest : BrowserDefault {
    [Fact]
    public async Task ShouldWork() {
        var plugin = new Languages();
        var page = await LaunchAndGetPage(plugin);

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
        var plugin = new Languages(new StealthLanguagesOptions("fr-FR"));
        var page = await LaunchAndGetPage(plugin);

        await page.GoToAsync("https://google.com");

        var fingerPrint = await FingerPrint.GetFingerPrint(page);

        var text = fingerPrint.GetRawText(); // for debug

        var languagesJson = fingerPrint.GetProperty("languages").GetRawText();

        var languages = JsonSerializer.Deserialize<string[]>(languagesJson);

        Assert.NotNull(languages);

        Assert.Contains("en-US", languages);
    }
}