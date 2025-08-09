using PuppeteerExtraSharpLite.Plugins.Stealth;

namespace PuppeteerExtraSharpLite.Tests.StealthPluginTests;

public partial class StealthPluginTests {
    private static readonly string[] Collection = ["function", "object"];

    [Fact]
    public async Task ChromeRuntime_Plugin_Should_AddChromeRuntime() {
        var pluginManager = new PluginManager();
        pluginManager.Register(new StealthPlugin()).Register(new ChromeRuntimePlugin());

        await using var browser = await pluginManager.LaunchAsync();
        var context = await browser.CreateBrowserContextAsync();
        await using var page = await context.NewPageAsync();

        await page.GoToAsync("https://google.com");

        var runtimeType = await page.EvaluateExpressionAsync<string>("typeof chrome.runtime");
        Assert.Equal("object", runtimeType);

        // At minimum the properties should exist (extension-like surface)
        var hasConnect = await page.EvaluateExpressionAsync<bool>("'connect' in chrome.runtime");
        var hasSendMessage = await page.EvaluateExpressionAsync<bool>("'sendMessage' in chrome.runtime");
        Assert.True(hasConnect);
        Assert.True(hasSendMessage);

        // Depending on environment, these may be real functions or null placeholders before proxying;
        // accept both "function" and "object" (null has typeof 'object').
        var connectType = await page.EvaluateExpressionAsync<string>("typeof chrome.runtime.connect");
        var sendMessageType = await page.EvaluateExpressionAsync<string>("typeof chrome.runtime.sendMessage");
        Assert.Contains(connectType, Collection);
        Assert.Contains(sendMessageType, Collection);
    }
}
