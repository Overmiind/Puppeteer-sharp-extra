using System.Text.Json;
using System.Threading.Tasks;
using Extra.Tests.Utils;
using PuppeteerExtraSharp.Plugins.ExtraStealth;
using Xunit;

namespace Extra.Tests.StealthPluginTests.EvasionsTests
{
    public class LoadTimesTest: BrowserDefault
    {
        [Fact]
        public async Task ShouldWork()
        {
            var stealthPlugin = new StealthPlugin();
            var page = await LaunchAndGetPageAsync(stealthPlugin);

            await page.GoToAsync("https://google.com");

            var loadTimes = await page.EvaluateFunctionAsync<JsonElement>("() => window.chrome.loadTimes()");

            Assert.NotNull(loadTimes);

            Assert.NotNull(loadTimes.GetString("connectionInfo"));
            Assert.NotNull(loadTimes.GetString("npnNegotiatedProtocol"));
            Assert.NotNull(loadTimes.GetProperty("wasAlternateProtocolAvailable"));
            Assert.NotNull(loadTimes.GetProperty("wasAlternateProtocolAvailable"));
            Assert.NotNull(loadTimes.GetProperty("wasFetchedViaSpdy"));
            Assert.NotNull(loadTimes.GetProperty("wasNpnNegotiated"));
            Assert.NotNull(loadTimes.GetProperty("firstPaintAfterLoadTime"));
            Assert.NotNull(loadTimes.GetProperty("requestTime"));
            Assert.NotNull(loadTimes.GetProperty("startLoadTime"));
            Assert.NotNull(loadTimes.GetProperty("commitLoadTime"));
            Assert.NotNull(loadTimes.GetProperty("finishDocumentLoadTime"));
            Assert.NotNull(loadTimes.GetProperty("firstPaintTime"));
        }
    }
}
