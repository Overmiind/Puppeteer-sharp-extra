using System.Text.Json;

using PuppeteerExtraSharpLite.Plugins.ExtraStealth.Evasions;

namespace PuppeteerExtraSharpLite.Tests.StealthPluginTests.EvasionsTests;

public class LoadTimesTest : BrowserDefault {
    [Fact]
    public async Task ShouldWork() {
        var stealthPlugin = new LoadTimes();
        var page = await LaunchAndGetPage(stealthPlugin);

        await page.GoToAsync("https://google.com");

        var loadTimes = await page.EvaluateFunctionAsync<JsonElement>("() => window.chrome.loadTimes()");

        Assert.NotNull(loadTimes.GetProperty("connectionInfo").GetString());
        Assert.NotNull(loadTimes.GetProperty("npnNegotiatedProtocol").GetString());
        Assert.NotNull(loadTimes.GetProperty("wasAlternateProtocolAvailable").GetString());
        Assert.NotNull(loadTimes.GetProperty("wasAlternateProtocolAvailable").GetString());
        Assert.NotNull(loadTimes.GetProperty("wasFetchedViaSpdy").GetString());
        Assert.NotNull(loadTimes.GetProperty("wasNpnNegotiated").GetString());
        Assert.NotNull(loadTimes.GetProperty("firstPaintAfterLoadTime").GetString());
        Assert.NotNull(loadTimes.GetProperty("requestTime").GetString());
        Assert.NotNull(loadTimes.GetProperty("startLoadTime").GetString());
        Assert.NotNull(loadTimes.GetProperty("commitLoadTime").GetString());
        Assert.NotNull(loadTimes.GetProperty("finishDocumentLoadTime").GetString());
        Assert.NotNull(loadTimes.GetProperty("firstPaintTime").GetString());
    }
}