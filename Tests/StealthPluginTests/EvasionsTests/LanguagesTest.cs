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
            var fingerprintLanguages = JArray.Parse(fingerPrint["languages"].Value<string>()).Select(t => t.Value<string>()).ToList();

            Assert.Contains("en-US", fingerprintLanguages);
        }  
        
        
        [Fact]
        public async Task ShouldWorkWithCustomSettings()
        {
            string[] langs = ["fr-FR"];
            var plugin = new Languages(new StealthLanguagesOptions("fr-FR"));
            var page = await LaunchAndGetPage(plugin);

            await page.GoToAsync("https://google.com");

            var fingerPrint = await FingerPrint.GetFingerPrint(page);
            var fingerprintLanguages = JArray.Parse(fingerPrint["languages"].Value<string>()).Select(t => t.Value<string>()).ToList();

            Assert.False(fingerprintLanguages.Except(langs).Any());
        }
    }
}
