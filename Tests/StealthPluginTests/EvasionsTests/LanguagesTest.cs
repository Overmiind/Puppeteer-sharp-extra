using System.Linq;
using System.Threading.Tasks;
using Extra.Tests.Utils;
using PuppeteerExtraSharp.Plugins.ExtraStealth;
using PuppeteerExtraSharp.Plugins.ExtraStealth.Evasions;
using Xunit;

namespace Extra.Tests.StealthPluginTests.EvasionsTests
{
    public class LanguagesTest : BrowserDefault
    {
        [Fact]
        public async Task ShouldWork()
        {
            var plugin = new Languages();
            var page = await LaunchAndGetPageAsync(plugin);

            await page.GoToAsync("https://google.com");

            var fingerPrint = await new FingerPrint().GetFingerPrint(page);

            Assert.Contains("en-US", fingerPrint.GetProperty("languages").EnumerateArray().Select(e => e.GetString()));
        }


        [Fact]
        public async Task ShouldWorkWithCustomSettings()
        {
            var stealthPlugin = new StealthPlugin(new StealthLanguagesOptions("fr-FR", "bl-BL"));

            var page = await LaunchAndGetPageAsync(stealthPlugin);

            await page.GoToAsync("https://google.com");

            var fingerPrint = await new FingerPrint().GetFingerPrint(page);

            Assert.Contains("fr-FR", fingerPrint.GetProperty("languages").EnumerateArray().Select(e => e.GetString()));
        }
    }
}