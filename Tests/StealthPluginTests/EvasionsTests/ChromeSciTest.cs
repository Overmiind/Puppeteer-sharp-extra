using System.Threading.Tasks;
using Extra.Tests.Utils;
using PuppeteerExtraSharp.Plugins.ExtraStealth;
using Xunit;

namespace Extra.Tests.StealthPluginTests.EvasionsTests
{
    public class ChromeSciTest : BrowserDefault
    {
        [Fact]
        public async Task ShouldWork()
        {
            var plugin = new StealthPlugin();
            var page = await LaunchAndGetPageAsync(plugin);
            await page.GoToAsync("https://google.com");
            var sci = (await page.EvaluateFunctionAsync(@"() => {
                            const { timing } = window.performance
                            const csi = window.chrome.csi()
                            return {
                              csi: {
                                exists: window.chrome && 'csi' in window.chrome,
                                toString: chrome.csi.toString()
                              },
                              dataOK: {
                                onloadT: csi.onloadT === timing.domContentLoadedEventEnd,
                                startE: csi.startE === timing.navigationStart,
                                pageT: Number.isFinite(csi.pageT),
                                tran: Number.isFinite(csi.tran)
                              }
                            }
                          }")).Value;

            Assert.True(sci.GetProperty("csi").GetBoolean("exists"));
            Assert.Equal("function () { [native code] }", sci.GetProperty("csi").GetString("toString"));;
            
            var dataProp = sci.GetProperty("dataOK");
            Assert.True(dataProp.GetBoolean("onloadT"));
            Assert.True(dataProp.GetBoolean("pageT"));
            Assert.True(dataProp.GetBoolean("startE"));
            Assert.True(dataProp.GetBoolean("tran"));
        }
    }
}