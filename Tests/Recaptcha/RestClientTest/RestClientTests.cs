using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Xunit;
using Xunit.Abstractions;
using RestClient = PuppeteerExtraSharp.Plugins.Recaptcha.RestClient.RestClient;

namespace Extra.Tests.Recaptcha.RestClientTest
{
    [Collection("Captcha")]
    public class RestClientTests(ITestOutputHelper output)
    {
        private readonly ITestOutputHelper output = output;

        [Fact]
        public async Task ShouldWorkPostWithJson()
        {
            var client = new RestClient("https://postman-echo.com");
            Dictionary<string, string> data = new()
            {
                ["test"] = "test"
            };
            
            var result = await client.PostWithJsonAsync<JObject>("post", data, new CancellationToken());

            output.WriteLine(result.ToString());
            var actual = result["json"].ToObject<Dictionary<string, string>>();

            Assert.Equal(data, actual);
        }

    }
}
