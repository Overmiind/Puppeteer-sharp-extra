using PuppeteerExtraSharpLite.Plugins.ExtraStealth.Evasions;
using PuppeteerExtraSharpLite.Tests.Utils;

namespace PuppeteerExtraSharpLite.Tests.StealthPluginTests.EvasionsTests;

public class UserAgentTest {
    [Fact]
    public async Task ShouldWork() {
        var pluginManager = new PluginManager();
        pluginManager.Register(new UserAgent());

        await using var browser = await pluginManager.LaunchAsync();
        using var page = await browser.NewPageAsync();

        await page.GoToAsync("https://google.com");
        var userAgent = await page.Browser.GetUserAgentAsync();

        var finger = await FingerPrint.GetFingerPrint(page);
        Assert.DoesNotContain("HeadlessChrome", finger.GetProperty("userAgent").GetString());
    }
}