using PuppeteerExtraSharpLite.Tests.Utils;

using PuppeteerExtraSharpLite.Plugins.ExtraStealth.Evasions;

namespace PuppeteerExtraSharpLite.Tests.StealthPluginTests.EvasionsTests;

public class CodecTest : BrowserDefault {
    [Fact]
    public async Task SupportsCodec() {
        var plugin = new Codec();
        var page = await LaunchAndGetPage(plugin);
        await page.GoToAsync("https://google.com");
        var fingerPrint = await FingerPrint.GetFingerPrint(page);

        var text = fingerPrint.GetRawText();

        Assert.Equal("", fingerPrint.GetProperty("videoCodecs").GetProperty("ogg").GetString());
        Assert.Equal("probably", fingerPrint.GetProperty("videoCodecs").GetProperty("h264").GetString());
        Assert.Equal("probably", fingerPrint.GetProperty("videoCodecs").GetProperty("webm").GetString());

        Assert.Equal("probably", fingerPrint.GetProperty("audioCodecs").GetProperty("ogg").GetString());
        Assert.Equal("probably", fingerPrint.GetProperty("audioCodecs").GetProperty("mp3").GetString());
        Assert.Equal("probably", fingerPrint.GetProperty("audioCodecs").GetProperty("wav").GetString());
        Assert.Equal("maybe", fingerPrint.GetProperty("audioCodecs").GetProperty("m4a").GetString());
        Assert.Equal("probably", fingerPrint.GetProperty("audioCodecs").GetProperty("aac").GetString());
    }

    [Fact]
    public async Task NotLeakModifications() {
        var plugin = new Codec();
        var page = await LaunchAndGetPage(plugin);

        var canPlay =
            await page.EvaluateFunctionAsync<string>(
                "() => document.createElement('audio').canPlayType.toString()");
        Assert.Equal("function canPlayType() { [native code] }", canPlay);

        var canPlayHasName = await page.EvaluateFunctionAsync<string>(
            "() => document.createElement('audio').canPlayType.name");
        Assert.Equal("canPlayType", canPlayHasName);
    }
}