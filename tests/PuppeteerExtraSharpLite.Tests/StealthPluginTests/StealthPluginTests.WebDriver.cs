using PuppeteerExtraSharpLite.Plugins;

namespace PuppeteerExtraSharpLite.Tests.StealthPluginTests;

public partial class StealthPluginTests {
    [Fact]
    public async Task WebDriver_Plugin_Test() {
        var pluginManager = new PluginManager();
        pluginManager.Register(new WebDriverPlugin());

        await using var browser = await pluginManager.LaunchAsync();
        var context = await browser.CreateBrowserContextAsync();
        await using var page = await context.NewPageAsync();

        await page.GoToAsync("https://google.com");

        var driver = await page.EvaluateExpressionAsync<bool>("navigator.webdriver");
        Assert.False(driver);
    }

    [Fact]
    public async Task WebDriver_Plugin_ShouldNot_KillOtherMethods() {
        var pluginManager = new PluginManager();
        pluginManager.Register(new WebDriverPlugin());

        await using var browser = await pluginManager.LaunchAsync();
        var context = await browser.CreateBrowserContextAsync();
        await using var page = await context.NewPageAsync();

        await page.GoToAsync("https://google.com");

        var data = await page.EvaluateExpressionAsync<bool>("navigator.javaEnabled()");
        Assert.False(data);
    }
}