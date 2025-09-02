using System.Linq;
using System.Threading.Tasks;
using Extra.Tests.Utils;
using PuppeteerExtraSharp.Plugins.ExtraStealth;
using Xunit;

namespace Extra.Tests.StealthPluginTests.EvasionsTests
{
    public class PluginTest : BrowserDefault
    {

        public async Task HasMimeTypes()
        {
            var plugin = new StealthPlugin();
            var page = await LaunchAndGetPageAsync(plugin);
            await page.GoToAsync("https://google.com");

            var finger = await new FingerPrint().GetFingerPrint(page);

            Assert.Equal(3, finger.GetProperty("plugins").EnumerateArray().Count());
            Assert.Equal(4, finger.GetProperty("mimeTypes").EnumerateArray().Count());
        }
    }
}
