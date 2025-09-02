using System.Text.Json;
using System.Threading.Tasks;
using PuppeteerExtraSharp.Plugins.ExtraStealth;
using Xunit;

namespace Extra.Tests.StealthPluginTests.EvasionsTests
{
    public class RuntimeTest : BrowserDefault
    {
        [Fact]
        public async Task ShouldAddConnectToChrome()
        {
            var plugin = new StealthPlugin();
            var page = await LaunchAndGetPageAsync(plugin);

            await page.GoToAsync("https://google.com");

            var runtime = await page.EvaluateExpressionAsync<JsonElement>("chrome.runtime");
            Assert.NotNull(runtime);

            var runtimeConnect = await page.EvaluateExpressionAsync<JsonElement>("chrome.runtime.connect");
            Assert.NotNull(runtimeConnect);

            var runtimeName = await page.EvaluateExpressionAsync<string>("chrome.runtime.connect.name");
            Assert.Equal("connect", runtimeName);

            var sendMessage = await page.EvaluateExpressionAsync<string>("chrome.runtime.sendMessage.name");
            Assert.NotNull(sendMessage);

            var sendMessageUndefined = await page.EvaluateExpressionAsync<bool>("chrome.runtime.sendMessage('nckgahadagoaajjgafhacjanaoiihapd', '') === undefined");
            Assert.True(sendMessageUndefined);

            var validIdWorks = await page.EvaluateExpressionAsync<bool>("chrome.runtime.connect('nckgahadagoaajjgafhacjanaoiihapd') !== undefined");
            Assert.True(validIdWorks);

            var nestedToString = await page.EvaluateExpressionAsync<string>("chrome.runtime.connect('nckgahadagoaajjgafhacjanaoiihapd').onDisconnect.addListener + ''");
            Assert.Equal("function addListener() { [native code] }", nestedToString);

            var noReturn = await page.EvaluateExpressionAsync<bool>("chrome.runtime.connect('nckgahadagoaajjgafhacjanaoiihapd').disconnect() === undefined");
            Assert.True(noReturn);
        }
    }
}
