using System.Net;

namespace PuppeteerExtraSharpLite.Tests;

public class ExtraLaunchTest : BrowserDefault {
    [Fact]
    public async Task ShouldReturnOkPage() {
        var browser = await this.LaunchAsync();
        var page = await browser.NewPageAsync();
        var response = await page.GoToAsync("http://google.com");
        Assert.Equal(HttpStatusCode.OK, response.Status);
        await browser.CloseAsync();
    }
}