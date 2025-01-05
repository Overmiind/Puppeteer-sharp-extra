using System.Linq;
using System.Threading.Tasks;
using Extra.Tests.Utils;
using PuppeteerExtraSharp.Plugins.ExtraStealth.Evasions;
using Xunit;

namespace Extra.Tests.StealthPluginTests.EvasionsTests;

public class PluginTest : BrowserDefault
{
    [Fact]
    public async Task HasMimeTypes()
    {
        var plugin = new PluginEvasion();
        var page = await LaunchAndGetPage(plugin);
        await page.GoToAsync("https://google.com");

        var finger = await new FingerPrint().GetFingerPrint(page);

        Assert.Equal(5, finger["plugins"].AsArray().Count);
        Assert.Equal(2, finger["mimeTypes"].AsArray().Count);
    }
}
