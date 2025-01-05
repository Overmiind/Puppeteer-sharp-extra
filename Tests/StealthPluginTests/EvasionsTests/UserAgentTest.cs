using System.Threading.Tasks;
using Extra.Tests.Utils;
using PuppeteerExtraSharp.Plugins.ExtraStealth.Evasions;
using Xunit;

namespace Extra.Tests.StealthPluginTests.EvasionsTests;

public class UserAgentTest : BrowserDefault
{
    [Fact]
    public async Task ShouldWork()
    {
        var plugin = new UserAgent();
        var page = await LaunchAndGetPage(plugin);
        await page.GoToAsync("https://google.com");
        var userAgent = await page.Browser.GetUserAgentAsync();

        var finger = await new FingerPrint().GetFingerPrint(page);
        Assert.DoesNotContain("HeadlessChrome", finger["userAgent"].GetValue<string>());
    }
}
