using System.Text.Json;

using PuppeteerExtraSharpLite.Plugins.ExtraStealth.Evasions;

using PuppeteerSharp;

namespace PuppeteerExtraSharpLite.Tests.StealthPluginTests.EvasionsTests;

public class ChromeAppTest {
    [Fact]
    public async Task ShouldWork() {
        var pluginManager = new PluginManager();
        pluginManager.Register(new ChromeApp());

        await using var browser = await pluginManager.LaunchAsync();
        using var page = await browser.NewPageAsync();

        await page.GoToAsync("https://google.com");

        var chrome = await page.EvaluateExpressionAsync<JsonElement>("window.chrome");

        var app = await page.EvaluateExpressionAsync<JsonElement>("chrome.app");

        var getIsInstalled = await page.EvaluateExpressionAsync<bool>("chrome.app.getIsInstalled()");
        Assert.False(getIsInstalled);

        var installState = await page.EvaluateExpressionAsync<JsonElement>("chrome.app.InstallState");
        Assert.Equal("disabled", installState.GetProperty("DISABLED").GetString());
        Assert.Equal("installed", installState.GetProperty("INSTALLED").GetString());
        Assert.Equal("not_installed", installState.GetProperty("NOT_INSTALLED").GetString());

        var runningState = await page.EvaluateExpressionAsync<JsonElement>("chrome.app.RunningState");
        Assert.Equal("cannot_run", runningState.GetProperty("CANNOT_RUN").GetString());
        Assert.Equal("ready_to_run", runningState.GetProperty("READY_TO_RUN").GetString());
        Assert.Equal("running", runningState.GetProperty("RUNNING").GetString());

        var details = await page.EvaluateExpressionAsync<object>("chrome.app.getDetails()");
        Assert.Null(details);

        var runningStateFunc = await page.EvaluateExpressionAsync<string>("chrome.app.runningState()");
        Assert.Equal("cannot_run", runningStateFunc);

        await Assert.ThrowsAsync<EvaluationFailedException>(async () => await page.EvaluateExpressionAsync("chrome.app.getDetails('foo')"));
    }
}