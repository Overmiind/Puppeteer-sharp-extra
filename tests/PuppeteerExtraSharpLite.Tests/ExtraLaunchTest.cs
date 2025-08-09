using System.Net;

namespace PuppeteerExtraSharpLite.Tests;

public class ExtraLaunchTest {
    [Fact]
    public async Task ShouldReturnOkPage() {
        var pluginManager = new PluginManager();

        await using var browser = await pluginManager.LaunchAsync();
        await using var page = await browser.NewPageAsync();

        var response = await page.GoToAsync("http://google.com");
        Assert.Equal(HttpStatusCode.OK, response.Status);
    }
}