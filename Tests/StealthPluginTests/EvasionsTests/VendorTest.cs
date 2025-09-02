using System.Threading.Tasks;
using PuppeteerExtraSharp.Plugins.ExtraStealth;
using Xunit;

namespace Extra.Tests.StealthPluginTests.EvasionsTests
{
    public class VendorTest: BrowserDefault
    {
        [Fact]
        public async Task ShouldWork()
        {
            var plugin = new StealthPlugin();
            var page = await LaunchAndGetPageAsync(plugin);
            await page.GoToAsync("https://google.com");

            var vendor = await page.EvaluateExpressionAsync<string>("navigator.vendor");
            Assert.Equal("Google Inc.", vendor);
        }
    }
}
