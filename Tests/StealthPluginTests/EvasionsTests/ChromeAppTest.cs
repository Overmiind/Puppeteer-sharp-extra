using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using PuppeteerExtraSharp.Plugins.ExtraStealth.Evasions;
using PuppeteerSharp;
using Xunit;

namespace Extra.Tests.StealthPluginTests.EvasionsTests;

public class ChromeAppTest : BrowserDefault
{
    [Fact]
    public async Task ShouldWork()
    {
        var plugin = new ChromeApp();

        var page = await LaunchAndGetPage(plugin);
        await page.GoToAsync("https://google.com");

        var chrome = await page.EvaluateExpressionAsync<JObject>("window.chrome");
        Assert.NotNull(chrome);

        var app = await page.EvaluateExpressionAsync<JObject>("chrome.app");
        Assert.NotNull(app);

        var getIsInstalled =
            await page.EvaluateExpressionAsync<bool>("chrome.app.getIsInstalled()");
        Assert.False(getIsInstalled);

        var installState = await page.EvaluateExpressionAsync<JObject>("chrome.app.InstallState");
        Assert.NotNull(installState);
        Assert.Equal("disabled", installState["DISABLED"]);
        Assert.Equal("installed", installState["INSTALLED"]);
        Assert.Equal("not_installed", installState["NOT_INSTALLED"]);

        var runningState = await page.EvaluateExpressionAsync<JObject>("chrome.app.RunningState");
        Assert.NotNull(runningState);
        Assert.Equal("cannot_run", runningState["CANNOT_RUN"]);
        Assert.Equal("ready_to_run", runningState["READY_TO_RUN"]);
        Assert.Equal("running", runningState["RUNNING"]);

        var details = await page.EvaluateExpressionAsync<object>("chrome.app.getDetails()");
        Assert.Null(details);

        var runningStateFunc =
            await page.EvaluateExpressionAsync<string>("chrome.app.runningState()");
        Assert.Equal("cannot_run", runningStateFunc);


        await Assert.ThrowsAsync<EvaluationFailedException>(async ()
            => await page.EvaluateExpressionAsync("chrome.app.getDetails('foo')"));
    }
}
