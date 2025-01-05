using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using JsonSerializer = System.Text.Json.JsonSerializer;
using RestClient = PuppeteerExtraSharp.Plugins.Recaptcha.RestClient.RestClient;

namespace Extra.Tests.Recaptcha.RestClientTest;

[Collection("Captcha")]
public class RestClientTests
{
    [Fact]
    public async Task ShouldWorkPostWithJson()
    {
        var client = new RestClient("https://postman-echo.com");
        var data = ("test", "123");

        var result =
            await client.PostWithJsonAsync<Dictionary<string, string>>("post",
                data,
                CancellationToken.None);

        var actual =
            JsonSerializer.Deserialize<(string, string)>(
                result.First(e => e.Key == "json").Value);

        Assert.Equal(data, actual);
    }
}
