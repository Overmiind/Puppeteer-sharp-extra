using System.Threading.Tasks;
using Extra.Tests.Utils;
using PuppeteerExtraSharp.Plugins.ExtraStealth.Evasions;
using Xunit;

namespace Extra.Tests.StealthPluginTests.EvasionsTests;

public class PermissionsTest : BrowserDefault
{
    [Fact]
    public async Task ShouldBeDeniedInHttpSite()
    {
        var launchOptions = CreateDefaultOptions();
        launchOptions.Args =
        [
            "--disable-features=HttpsUpgrades"
        ];
        
        var plugin = new Permissions();
        var page = await LaunchAndGetPage(plugin, launchOptions);
        await page.GoToAsync("http://info.cern.ch/");

        var finger = await new FingerPrint().GetFingerPrint(page);

        Assert.Equal("http://info.cern.ch/", page.Url);
        Assert.Equal("denied", finger["permissions"]["state"].GetValue<string>());
        Assert.Equal("denied", finger["permissions"]["permission"].GetValue<string>());
    }
}
