using System.Linq;
using System.Threading.Tasks;
using Extra.Tests.Utils;
using Newtonsoft.Json.Linq;
using PuppeteerExtraSharp.Plugins.ExtraStealth.Evasions;
using Xunit;

namespace Extra.Tests.StealthPluginTests.EvasionsTests
{
    public class LanguagesTest: BrowserDefault
    {
        [Fact]
        public async Task ShouldWork()
        {
            var plugin = new Languages();
            var page = await LaunchAndGetPage(plugin);

            await page.GoToAsync("https://google.com");

            var fingerPrint = await FingerPrint.GetFingerPrint(page);

            Assert.Contains("en-US", fingerPrint["languages"].Select(e => e.Value<string>()));
        }  
        
        
        [Fact]
        public async Task ShouldWorkWithCustomSettings()
        {
            var plugin = new Languages(new StealthLanguagesOptions("fr-FR"));
            var page = await LaunchAndGetPage(plugin);

            await page.GoToAsync("https://google.com");

            var fingerPrint = await FingerPrint.GetFingerPrint(page);

            Assert.Contains("fr-FR", fingerPrint["languages"].Select(e => e.Value<string>()));
        }
    }
}
