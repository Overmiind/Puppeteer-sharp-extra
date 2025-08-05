using PuppeteerExtraSharpLite.Plugins.ExtraStealth.Evasions;

namespace PuppeteerExtraSharpLite.Tests.StealthPluginTests.EvasionsTests;

public class WebDriverTest {
    [Fact]
    public async Task ShouldWork() {
        var pluginManager = new PluginManager();
        pluginManager.Register(new WebDriver());

        await using var browser = await pluginManager.LaunchAsync();
        using var page = await browser.NewPageAsync();

        await page.GoToAsync("https://google.com");

        var driver = await page.EvaluateExpressionAsync<bool>("navigator.webdriver");
        Assert.False(driver);
    }

    [Fact]
    public async Task WontKillOtherMethods() {
        var pluginManager = new PluginManager();
        pluginManager.Register(new WebDriver());

        await using var browser = await pluginManager.LaunchAsync();
        using var page = await browser.NewPageAsync();

        await page.GoToAsync("https://google.com");

        var data = await page.EvaluateExpressionAsync<bool>("navigator.javaEnabled()");
        Assert.False(data);
    }
}