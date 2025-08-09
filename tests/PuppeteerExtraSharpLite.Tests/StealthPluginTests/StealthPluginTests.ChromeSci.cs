using System.Text.Json;

using PuppeteerExtraSharpLite.Plugins.Stealth;

namespace PuppeteerExtraSharpLite.Tests.StealthPluginTests;

public partial class StealthPluginTests {
    [Fact]
    public async Task ChromeSci_Plugin_Test() {
        var pluginManager = new PluginManager();
        pluginManager.Register(new StealthPlugin()).Register(new ChromeSciPlugin());

        await using var browser = await pluginManager.LaunchAsync();
        var context = await browser.CreateBrowserContextAsync();
        await using var page = await context.NewPageAsync();

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

        var text = sci.GetRawText(); // for debug

        Assert.True(sci.GetProperty("csi").GetProperty("exists").GetBoolean());
        Assert.Equal("function () { [native code] }", sci.GetProperty("csi").GetProperty("toString").GetString());
        Assert.True(sci.GetProperty("dataOK").GetProperty("onloadT").GetBoolean());
        Assert.True(sci.GetProperty("dataOK").GetProperty("startE").GetBoolean());
        Assert.False(sci.GetProperty("dataOK").GetProperty("pageT").GetBoolean());
        Assert.True(sci.GetProperty("dataOK").GetProperty("tran").GetBoolean());
    }
}