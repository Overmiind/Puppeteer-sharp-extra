using System.Text.Json;

using PuppeteerExtraSharpLite.Plugins.ExtraStealth;

using PuppeteerExtraSharpLite.Plugins.ExtraStealth.Evasions;

namespace PuppeteerExtraSharpLite.Tests.StealthPluginTests.EvasionsTests;

public class LoadTimesTest {
    [Fact]
    public async Task ShouldWork() {
        var pluginManager = new PluginManager();
        pluginManager.Register(new StealthPlugin()).Register(new LoadTimes());

        await using var browser = await pluginManager.LaunchAsync();
        using var page = await browser.NewPageAsync();

        await page.GoToAsync("https://google.com");

        var loadTimes = await page.EvaluateFunctionAsync<JsonElement>("() => window.chrome.loadTimes()");

        Assert.True(loadTimes.TryGetProperty("connectionInfo", out var prop));
        Assert.Equal(JsonValueKind.String, prop.ValueKind);

        Assert.True(loadTimes.TryGetProperty("npnNegotiatedProtocol", out prop));
        Assert.Equal(JsonValueKind.String, prop.ValueKind);

        Assert.True(loadTimes.TryGetProperty("wasAlternateProtocolAvailable", out prop));
        Assert.True(prop.ValueKind is JsonValueKind.True or JsonValueKind.False);

        Assert.True(loadTimes.TryGetProperty("wasFetchedViaSpdy", out prop));
        Assert.True(prop.ValueKind is JsonValueKind.True or JsonValueKind.False);

        Assert.True(loadTimes.TryGetProperty("wasNpnNegotiated", out prop));
        Assert.True(prop.ValueKind is JsonValueKind.True or JsonValueKind.False);

        Assert.True(loadTimes.TryGetProperty("firstPaintAfterLoadTime", out prop));
        Assert.Equal(JsonValueKind.Number, prop.ValueKind);

        Assert.True(loadTimes.TryGetProperty("requestTime", out prop));
        Assert.Equal(JsonValueKind.Number, prop.ValueKind);

        Assert.True(loadTimes.TryGetProperty("startLoadTime", out prop));
        Assert.Equal(JsonValueKind.Number, prop.ValueKind);

        Assert.True(loadTimes.TryGetProperty("commitLoadTime", out prop));
        Assert.Equal(JsonValueKind.Number, prop.ValueKind);

        Assert.True(loadTimes.TryGetProperty("finishDocumentLoadTime", out prop));
        Assert.Equal(JsonValueKind.Number, prop.ValueKind);

        Assert.True(loadTimes.TryGetProperty("firstPaintTime", out prop));
        Assert.Equal(JsonValueKind.Number, prop.ValueKind);
    }
}