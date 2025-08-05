using PuppeteerExtraSharpLite.Plugins.ExtraStealth.Evasions;

using PuppeteerSharp;

namespace PuppeteerExtraSharpLite.Tests.StealthPluginTests.EvasionsTests;

public class RuntimeTest {
    [Fact]
    public async Task ShouldAddConnectToChrome() {
        var pluginManager = new PluginManager();
        pluginManager.Register(new ChromeRuntime());

        await using var browser = await pluginManager.LaunchAsync();
        using var page = await browser.NewPageAsync();

        await page.GoToAsync("https://google.com");

        var runtimeType = await page.EvaluateExpressionAsync<string>("typeof chrome.runtime");

        Assert.Equal("object", runtimeType);

        //TODO: activate tests
        // return; // ignore the rest of the test for now
        // // TestContext.Current.SendDiagnosticMessage($"chrome.runtime typeof: {runtimeType}");

        // var runtime = await page.EvaluateExpressionAsync<JsonElement>("chrome.runtime");

        // var runtimeConnect = await page.EvaluateExpressionAsync<JsonElement>("chrome.runtime.connect");

        // var runtimeName = await page.EvaluateExpressionAsync<string>("chrome.runtime.connect.name");
        // Assert.Equal("connect", runtimeName);

        // var sendMessage = await page.EvaluateExpressionAsync<string>("chrome.runtime.sendMessage.name");
        // Assert.NotNull(sendMessage);

        // var sendMessageUndefined = await page.EvaluateExpressionAsync<bool>("chrome.runtime.sendMessage('nckgahadagoaajjgafhacjanaoiihapd', '') === undefined");
        // Assert.True(sendMessageUndefined);

        // var validIdWorks = await page.EvaluateExpressionAsync<bool>("chrome.runtime.connect('nckgahadagoaajjgafhacjanaoiihapd') !== undefined");
        // Assert.True(validIdWorks);

        // var nestedToString = await page.EvaluateExpressionAsync<string>("chrome.runtime.connect('nckgahadagoaajjgafhacjanaoiihapd').onDisconnect.addListener + ''");
        // Assert.Equal("function addListener() { [native code] }", nestedToString);

        // var noReturn = await page.EvaluateExpressionAsync<bool>("chrome.runtime.connect('nckgahadagoaajjgafhacjanaoiihapd').disconnect() === undefined");
        // Assert.True(noReturn);


        // await AssertThrowsConnect(page, "chrome.runtime.connect() called from a webpage must specify an Extension ID (string) for its first argument.", "");
        // await AssertThrowsConnect(page, "No matching signature.", "", "", "", "", "", "", "");
        // await AssertThrowsConnect(page, "Invalid extension id: 'foo'", "", "foo");
        // await AssertThrowsConnect(page, "Error at property 'includeTlsChannelId': Invalid type: expected boolean, found number.", "", new { IncludeTlsChannelId = 777 });
    }


    private async Task AssertThrowsConnect(IPage page, string error, params object[] args) {
        var start =
            "Evaluation failed: TypeError: Error in invocation of runtime.connect(optional string extensionId, optional object connectInfo): ";
        var ex = await Assert.ThrowsAsync<EvaluationFailedException>(async () =>
            await page.EvaluateFunctionAsync("(...args) => chrome.runtime.connect.call(...args)", args));

        var currentError = start + error;
        Assert.StartsWith(currentError, ex.Message);
    }
}
