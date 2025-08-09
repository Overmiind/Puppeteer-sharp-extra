using System.Text.Json;

using PuppeteerExtraSharpLite.Plugins.Stealth;

namespace PuppeteerExtraSharpLite.Tests.StealthPluginTests;

public partial class StealthPluginTests {
    [Fact]
    public async Task ChromeRuntime_Plugin_Should_AddConnectToChrome() {
        var pluginManager = new PluginManager();
        pluginManager.Register(new StealthPlugin()).Register(new ChromeRuntimePlugin());

        await using var browser = await pluginManager.LaunchAsync();
        await using var page = await browser.NewPageAsync();

        await page.GoToAsync("https://google.com");

        var runtimeType = await page.EvaluateExpressionAsync<string>("typeof chrome.runtime");

        Assert.Equal("object", runtimeType);

        var runtime = await page.EvaluateExpressionAsync<JsonElement>("chrome.runtime");

        // Further asserts that treated chrome.runtime.connect as a function were removed
        // Perhaps due to browser updates - chrome.runtime.connect is only a property and most of the =null
        Assert.Null(await page.EvaluateExpressionAsync<string?>("chrome.runtime.connect"));

        Assert.Null(await page.EvaluateExpressionAsync<string?>("chrome.runtime.sendMessage"));
    }
}
