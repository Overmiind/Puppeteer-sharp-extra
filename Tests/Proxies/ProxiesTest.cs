using System.Threading.Tasks;
using Xunit.Abstractions;
using Xunit;
using PuppeteerExtraSharp.Plugins.Proxies;
using Extra.Tests.Properties;

namespace Extra.Tests.Proxies
{
    [Collection("Proxies")]
    public class ProxiesTest(ITestOutputHelper output) : BrowserDefault
    {
        private readonly ITestOutputHelper output = output;

        [Fact]
        public async Task ShouldWorkWithProxy()
        {
            var plugin = new ProxiesPlugin(new(Resources.ProxyIp, Resources.ProxyPort, Resources.ProxyLogin, Resources.ProxyPassword));
            var browser = await LaunchWithPluginAsync(plugin);

            var page = await browser.NewPageAsync();
            await page.GoToAsync("http://api.ipify.org");
            string content = await page.EvaluateExpressionAsync<string>("document.body.innerText");
            output.WriteLine(content);
            await browser.CloseAsync();

            Assert.Equal(content, Resources.ProxyIp);
        }
    }
}
