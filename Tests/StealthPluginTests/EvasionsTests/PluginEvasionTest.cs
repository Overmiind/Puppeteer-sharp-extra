using System.Linq;
using System.Threading.Tasks;
using Extra.Tests.Utils;
using PuppeteerExtraSharp.Plugins.ExtraStealth.Evasions;
using Xunit;

namespace Extra.Tests.StealthPluginTests.EvasionsTests;

public class PluginEvasionTest : BrowserDefault
{
    [Fact]
    public async Task ShouldNotHaveModifications()
    {
        var stealthPlugin = new PluginEvasion();
        var page = await LaunchAndGetPage(stealthPlugin);

        await page.GoToAsync("https://google.com");

        var fingerPrint = await new FingerPrint().GetFingerPrint(page);

        Assert.Equal(5, fingerPrint["plugins"].AsArray().Count);
        Assert.Equal(2, fingerPrint["mimeTypes"].AsArray().Count);
    }
}
