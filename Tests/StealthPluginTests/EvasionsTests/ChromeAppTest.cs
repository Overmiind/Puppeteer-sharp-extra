using System.Text.Json.Nodes;
using System.Threading.Tasks;
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

        var chrome = await page.EvaluateExpressionAsync<JsonObject>("window.chrome");
        Assert.NotNull(chrome);

        var app = await page.EvaluateExpressionAsync<JsonObject>("chrome.app");
        Assert.NotNull(app);

        var getIsInstalled =
            await page.EvaluateExpressionAsync<bool>("chrome.app.getIsInstalled()");
        Assert.False(getIsInstalled);

        var installState = await page.EvaluateExpressionAsync<JsonObject>("chrome.app.InstallState");
        Assert.NotNull(installState);
        Assert.Equal("disabled", installState["DISABLED"].GetValue<string>());
        Assert.Equal("installed", installState["INSTALLED"].GetValue<string>());
        Assert.Equal("not_installed", installState["NOT_INSTALLED"].GetValue<string>());

        var runningState = await page.EvaluateExpressionAsync<JsonObject>("chrome.app.RunningState");
        Assert.NotNull(runningState);
        Assert.Equal("cannot_run", runningState["CANNOT_RUN"].GetValue<string>());
        Assert.Equal("ready_to_run", runningState["READY_TO_RUN"].GetValue<string>());
        Assert.Equal("running", runningState["RUNNING"].GetValue<string>());

        var details = await page.EvaluateExpressionAsync<object>("chrome.app.getDetails()");
        Assert.Null(details);

        var runningStateFunc =
            await page.EvaluateExpressionAsync<string>("chrome.app.runningState()");
        Assert.Equal("cannot_run", runningStateFunc);

        await Assert.ThrowsAsync<EvaluationFailedException>(async ()
            => await page.EvaluateExpressionAsync("chrome.app.getDetails('foo')"));
    }
}
