using System.Net;

namespace PuppeteerSharpToolkit.Tests;

public class ExtraLaunchTest {
    [Fact]
    public async Task ShouldReturnOkPage() {
        var pluginManager = new PluginManager();

        await using var browser = await pluginManager.LaunchAsync();
        var context = await browser.CreateBrowserContextAsync();
        await using var page = await context.NewPageAsync();

        var response = await page.GoToAsync("http://google.com");
        Assert.Equal(HttpStatusCode.OK, response.Status);
    }
}
