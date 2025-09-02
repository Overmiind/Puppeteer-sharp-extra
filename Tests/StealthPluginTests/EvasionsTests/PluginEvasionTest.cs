using System.Linq;
using System.Threading.Tasks;
using Extra.Tests.Utils;
using PuppeteerExtraSharp.Plugins.ExtraStealth;
using Xunit;

namespace Extra.Tests.StealthPluginTests.EvasionsTests
{
    public class PluginEvasionTest : BrowserDefault
    {
        public async Task ShouldNotHaveModifications()
        {
            var stealthPlugin = new StealthPlugin();
            var page = await LaunchAndGetPageAsync(stealthPlugin);

            await page.GoToAsync("https://google.com");


            var fingerPrint = await new FingerPrint().GetFingerPrint(page);

            Assert.Equal(3, fingerPrint.GetProperty("plugins").EnumerateArray().Count());
            Assert.Equal(4, fingerPrint.GetProperty("mimeTypes").EnumerateArray().Count());
        }
    }
}