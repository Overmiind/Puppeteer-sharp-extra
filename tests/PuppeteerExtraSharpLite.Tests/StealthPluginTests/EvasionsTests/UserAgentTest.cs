using PuppeteerExtraSharpLite.Tests.Utils;

using PuppeteerExtraSharpLite.Plugins.ExtraStealth.Evasions;

namespace PuppeteerExtraSharpLite.Tests.StealthPluginTests.EvasionsTests;

public class UserAgentTest : BrowserDefault {
    [Fact]
    public async Task ShouldWork() {
        var plugin = new UserAgent();
        var page = await LaunchAndGetPage(plugin);
        await page.GoToAsync("https://google.com");
        var userAgent = await page.Browser.GetUserAgentAsync();

        var finger = await FingerPrint.GetFingerPrint(page);
        Assert.DoesNotContain("HeadlessChrome", finger.GetProperty("userAgent").GetString());
    }
}