using System.Threading.Tasks;
using PuppeteerExtraSharp.Plugins.ExtraStealth.Evasions;
using Xunit;

namespace Extra.Tests.StealthPluginTests.EvasionsTests;

public class WebDriverTest : BrowserDefault
{
    [Fact]
    public async Task ShouldWork()
    {
        var plugin = new WebDriver();
        var page = await LaunchAndGetPage(plugin);
        await page.GoToAsync("https://google.com");

        var driver = await page.EvaluateExpressionAsync<bool>("navigator.webdriver");
        Assert.False(driver);
    }

    [Fact]
    public async Task WontKillOtherMethods()
    {
        var plugin = new WebDriver();
        var page = await LaunchAndGetPage(plugin);
        await page.GoToAsync("https://google.com");

        var data = await page.EvaluateExpressionAsync<bool>("navigator.javaEnabled()");
        Assert.False(data);
    }
}
