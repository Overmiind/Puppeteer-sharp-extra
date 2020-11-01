using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Extra.Tests.Utils;
using Newtonsoft.Json.Linq;
using PuppeteerExtraSharp.Plugins.ExtraStealth.Evasions;
using Xunit;

namespace Extra.Tests.StealthPluginTests
{
    public class LanguagesTest: BrowserDefault
    {
        [Fact]
        public async Task ShouldWork()
        {
            var plugin = new Languages();
            var page = await LaunchAndGetPage(plugin);

            await page.GoToAsync("https://google.com");

            var fingerPrint = await new FingerPrint().GetFingerPrint(page);

            Assert.Contains("en-US", fingerPrint["languages"].Select(e => e.Value<string>()));

            var property = await page.EvaluateExpressionAsync("Object.getOwnPropertyDescriptor(navigator, 'languages')");
            Assert.Null(property);

            var navigator = await page.EvaluateExpressionAsync("navigator");
            Assert.Empty(navigator);
        }
    }
}
