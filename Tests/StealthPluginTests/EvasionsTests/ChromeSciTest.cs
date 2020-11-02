using System.Threading.Tasks;
using PuppeteerExtraSharp.Plugins.ExtraStealth.Evasions;
using Xunit;

namespace Extra.Tests.StealthPluginTests.EvasionsTests
{
    public class ChromeSciTest : BrowserDefault
    {
        [Fact]
        public async Task ShouldWork()
        {
            var plugin = new ChromeSci();
            var page = await LaunchAndGetPage(plugin);
            await page.GoToAsync("https://google.com");
            var sci = await page.EvaluateExpressionAsync("window.chrome.csi()");
            Assert.NotNull(sci);

            var onLoad =
                await page.EvaluateExpressionAsync<bool>(
                    "window.chrome.csi().onLoadT === window.performance.domContentLoadedEventEnd");
            Assert.True(onLoad);

            //var startE =
            //    await page.EvaluateExpressionAsync<bool>(
            //        "window.chrome.csi().startE === window.performance.navigationStart");

            //Assert.True(startE);

            var pageT = await page.EvaluateExpressionAsync<bool>("Number.isInteger(window.chrome.csi().pageT)");
            Assert.True(pageT);


            var tran = await page.EvaluateExpressionAsync<bool>("Number.isInteger(window.chrome.csi().tran)");
            Assert.True(tran);
        }
    }
}
