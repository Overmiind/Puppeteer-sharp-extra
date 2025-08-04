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
                                                    JsonContext.Default.JsonElement,
                                                    TestContext.Current.CancellationToken);


        var json = result.GetProperty("json").GetRawText();
        var inner = JsonSerializer.Deserialize(json, JsonContext.Default.DictionaryStringString);

        Assert.NotNull(inner);
        Assert.Equal(data, inner);
    }
}