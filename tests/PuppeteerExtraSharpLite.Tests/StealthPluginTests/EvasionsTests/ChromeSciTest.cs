using System.Text.Json;

using PuppeteerExtraSharpLite.Plugins.ExtraStealth.Evasions;

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
                          }") ?? new JsonElement();

        Assert.True(sci.GetProperty("csi").GetProperty("exists").GetBoolean());
        Assert.Equal("function () { [native code] }", sci.GetProperty("csi").GetProperty("toString").GetString());
        Assert.True(sci.GetProperty("dataOK").GetProperty("onloadT").GetBoolean());
        Assert.True(sci.GetProperty("dataOK").GetProperty("pageT").GetBoolean());
        Assert.True(sci.GetProperty("dataOK").GetProperty("startE").GetBoolean());
        Assert.True(sci.GetProperty("dataOK").GetProperty("tran").GetBoolean());
    }
}