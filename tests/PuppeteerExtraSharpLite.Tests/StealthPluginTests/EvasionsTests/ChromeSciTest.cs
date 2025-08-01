using System.Threading.Tasks;

using PuppeteerExtraSharp.Plugins.ExtraStealth.Evasions;

using Xunit;

namespace PuppeteerExtraSharpLite.Tests.StealthPluginTests.EvasionsTests;

public class ChromeSciTest : BrowserDefault {
  [Fact]
  public async Task ShouldWork() {
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

    Assert.True(sci["csi"].Value<bool>("exists"));
    Assert.Equal("function () { [native code] }", sci["csi"]["toString"]);
    Assert.True(sci["dataOK"].Value<bool>("onloadT"));
    Assert.True(sci["dataOK"].Value<bool>("pageT"));
    Assert.True(sci["dataOK"].Value<bool>("startE"));
    Assert.True(sci["dataOK"].Value<bool>("tran"));
  }
}