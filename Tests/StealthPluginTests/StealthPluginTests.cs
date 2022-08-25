using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using PuppeteerExtraSharp.Plugins.ExtraStealth;
using PuppeteerExtraSharp.Plugins.ExtraStealth.Evasions;
using Xunit;

namespace Extra.Tests.StealthPluginTests
{
    public class StealthPluginTests: BrowserDefault
    {
        [Fact]
        public async Task ShouldBeNotDetected()
        {
            var plugin = new StealthPlugin();
            plugin.RemoveEvasionByType<PluginEvasion>();
            plugin.RemoveEvasionByType<ContentWindow>();
            var page = await LaunchAndGetPage(plugin);
            await page.GoToAsync("https://google.com");

            var webdriver = await page.EvaluateExpressionAsync<bool>("navigator.webdriver");
            Assert.False(webdriver);

            var headlessUserAgent = await page.EvaluateExpressionAsync<string>("window.navigator.userAgent");
            Assert.DoesNotContain("Headless", headlessUserAgent);

            var webDriverOverriden =
                await page.EvaluateExpressionAsync<bool>(
                    "Object.getOwnPropertyDescriptor(navigator.__proto__, 'webdriver') !== undefined");
            Assert.True(webDriverOverriden);

            var plugins = await page.EvaluateExpressionAsync<int>("navigator.plugins.length");
            Assert.NotEqual(0, plugins);
        }
    }
}
