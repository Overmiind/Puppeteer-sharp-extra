using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Newtonsoft.Json;

using Xunit;

using RestClient = PuppeteerExtraSharp.Plugins.Recaptcha.RestClient.RestClient;

namespace PuppeteerExtraSharpLite.Tests.Recaptcha.RestClientTest;

[Collection("Captcha")]
public class RestClientTests {
    [Fact]
    public async Task ShouldWorkPostWithJson() {
        var client = new RestClient("https://postman-echo.com");
        var data = ("test", "123");

        var result = await client.PostWithJsonAsync<Dictionary<string, string>>("post", data, new CancellationToken());

        var actual = JsonConvert.DeserializeObject<(string, string)>(result.First(e => e.Key == "json").Value);

        Assert.Equal(data, actual);
    }

}