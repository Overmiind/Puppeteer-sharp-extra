using System.Text.Json;

using PuppeteerExtraSharpLite.Plugins.Recaptcha.RestClient;

using RestClient = PuppeteerExtraSharpLite.Plugins.Recaptcha.RestClient.RestClient;

namespace PuppeteerExtraSharpLite.Tests.Recaptcha.RestClientTest;

[Collection("Captcha")]
public class RestClientTests {
    [Fact]
    public async Task ShouldWorkPostWithJson() {
        using var client = new RestClient("https://postman-echo.com");
        Dictionary<string, string> data = new() {
            ["test"] = "123"
        };

        var result = await client.PostWithJsonAsync("post",
                                                    data,
                                                    JsonContext.Default.DictionaryStringString,
                                                    JsonContext.Default.DictionaryStringString,
                                                    TestContext.Current.CancellationToken);

        Assert.NotNull(result);

        var inner = JsonSerializer.Deserialize(data["json"], JsonContext.Default.DictionaryStringString);

        Assert.NotNull(inner);
        Assert.Equal(data, inner);
    }

}