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
            var sci = await page.EvaluateFunctionAsync(@"() => {
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
                                pageT: Number.isInteger(csi.pageT),
                                tran: Number.isInteger(csi.tran)
                              }
                            }
                          }");
            
            Assert.NotNull(sci);

            var obj = sci.Value;
            var dataOk = obj.GetProperty("dataOK");
            
            Assert.True(obj.GetProperty("csi").GetProperty("exists").GetBoolean());
            Assert.True(dataOk.GetProperty("onloadT").GetBoolean());
            Assert.True(dataOk.GetProperty("pageT").GetBoolean());
            Assert.True(dataOk.GetProperty("startE").GetBoolean());
            Assert.True(dataOk.GetProperty("tran").GetBoolean());
        }
    }
}