using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using PuppeteerExtraSharp.Plugins.ExtraStealth.Evasions;
using PuppeteerSharp;
using Xunit;

namespace Extra.Tests.StealthPluginTests.EvasionsTests
{
    public class RuntimeTest : BrowserDefault
    {
        [Fact]
        public async Task ShouldAddConnectToChrome()
        {
            var plugin = new ChromeRuntime();
            var page = await LaunchAndGetPage(plugin);

            await page.GoToAsync("https://google.com");

            var runtime = await page.EvaluateExpressionAsync<JObject>("chrome.runtime");
            Assert.NotNull(runtime);

            var runtimeConnect = await page.EvaluateExpressionAsync<JObject>("chrome.runtime.connect");
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


            await AssertThrowsConnect(page, "chrome.runtime.connect() called from a webpage must specify an Extension ID (string) for its first argument.", "");
            await AssertThrowsConnect(page, "No matching signature.", "", "", "", "", "", "", "");
            await AssertThrowsConnect(page, "Invalid extension id: 'foo'", "", "foo");
            await AssertThrowsConnect(page, "Error at property 'includeTlsChannelId': Invalid type: expected boolean, found number.", "", new { IncludeTlsChannelId = 777 });
        }


        private async Task AssertThrowsConnect(IPage page, string error, params object[] args)
        {
            var start =
                "Evaluation failed: TypeError: Error in invocation of runtime.connect(optional string extensionId, optional object connectInfo): ";
            var ex = await Assert.ThrowsAsync<EvaluationFailedException>(async () =>
                await page.EvaluateFunctionAsync("(...args) => chrome.runtime.connect.call(...args)", args));

            var currentError = start + error;
            Assert.StartsWith(currentError, ex.Message);
        }
    }
}
