using System.Text.Json;
using System.Threading.Tasks;
using Extra.Tests.Utils;
using PuppeteerExtraSharp.Plugins.ExtraStealth;
using PuppeteerSharp;
using Xunit;

namespace Extra.Tests.StealthPluginTests.EvasionsTests
{
    public class ChromeAppTest : BrowserDefault
    {
        [Fact]
        public async Task ShouldWork()
        {
            var plugin = new StealthPlugin();

            var page = await LaunchAndGetPageAsync(plugin);
            await page.GoToAsync("https://google.com");

            var test = await page.EvaluateExpressionAsync<dynamic>("window.chrome");
            var chrome = await page.EvaluateExpressionAsync<JsonElement>("window.chrome");
            Assert.NotNull(chrome);

            var app = await page.EvaluateExpressionAsync<JsonElement>("chrome.app");
            Assert.NotNull(app);

            var getIsInstalled = await page.EvaluateExpressionAsync<bool>("chrome.app.getIsInstalled()");
            Assert.False(getIsInstalled);

            var installState = await page.EvaluateExpressionAsync<JsonElement>("chrome.app.InstallState");
            Assert.NotNull(installState);
            
            Assert.Equal("disabled", installState.GetString("DISABLED"));
            Assert.Equal("installed", installState.GetString("INSTALLED"));
            Assert.Equal("not_installed", installState.GetString("NOT_INSTALLED"));

            var runningState = await page.EvaluateExpressionAsync<JsonElement>("chrome.app.RunningState");
            Assert.NotNull(runningState);
            Assert.Equal("cannot_run", runningState.GetString("CANNOT_RUN"));
            Assert.Equal("ready_to_run", runningState.GetString("READY_TO_RUN"));
            Assert.Equal("running", runningState.GetString("RUNNING"));;

            var details = await page.EvaluateExpressionAsync<object>("chrome.app.getDetails()");
            Assert.Null(details);

            var runningStateFunc = await page.EvaluateExpressionAsync<string>("chrome.app.runningState()");
            Assert.Equal("cannot_run", runningStateFunc);


            await Assert.ThrowsAsync<EvaluationFailedException>(async () => await page.EvaluateExpressionAsync("chrome.app.getDetails('foo')"));
        }
    }
}
