using System.Threading.Tasks;
using PuppeteerExtraSharp.Plugins.ExtraStealth;
using PuppeteerExtraSharp.Plugins.ExtraStealth.Evasions;
using PuppeteerSharp;
using Xunit;

namespace Extra.Tests.StealthPluginTests.EvasionsTests
{
    public class StealthPluginTest: BrowserDefault
    {
        [Fact]
        public async Task Test()
        {

            var plugin = new StealthPlugin();
            plugin.RemoveEvasionByType<PluginEvasion>();
            plugin.RemoveEvasionByType<ContentWindow>();
            var browser = await LaunchWithPluginAsync(plugin);

            var page = await browser.NewPageAsync();
            await page.GoToAsync("https://bot.sannysoft.com");
            await page.ScreenshotAsync("Stealth.png", new ScreenshotOptions()
            {
                FullPage = true,
                Type = ScreenshotType.Png,
            });
        }
    }
}
