using System.Net;
using Xunit;

namespace Extra.Tests;

public class ExtraLaunchTest : BrowserDefault
{
    [Fact]
    public async void ShouldReturnOkPage()
    {
        var browser = await LaunchAsync();
        var page = await browser.NewPageAsync();
        var response = await page.GoToAsync("http://google.com");
        Assert.Equal(HttpStatusCode.OK, response.Status);
        await browser.CloseAsync();
    }
}
