using PuppeteerExtraSharpLite.Tests.Utils;

using Newtonsoft.Json.Linq;

using PuppeteerExtraSharpLite.Plugins.ExtraStealth.Evasions;

namespace PuppeteerExtraSharpLite.Tests.StealthPluginTests.EvasionsTests;

public class CodecTest : BrowserDefault {
    [Fact]
    public async Task SupportsCodec() {
        var plugin = new Codec();
        var page = await LaunchAndGetPage(plugin);
        await page.GoToAsync("https://google.com");
        var fingerPrint = await new FingerPrint().GetFingerPrint(page);

        Assert.Equal("probably", fingerPrint["videoCodecs"]["ogg"].Value<string>());
        Assert.Equal("probably", fingerPrint["videoCodecs"]["h264"].Value<string>());
        Assert.Equal("probably", fingerPrint["videoCodecs"]["webm"].Value<string>());

        Assert.Equal("probably", fingerPrint["audioCodecs"]["ogg"].Value<string>());
        Assert.Equal("probably", fingerPrint["audioCodecs"]["mp3"].Value<string>());
        Assert.Equal("probably", fingerPrint["audioCodecs"]["wav"].Value<string>());
        Assert.Equal("maybe", fingerPrint["audioCodecs"]["m4a"].Value<string>());
        Assert.Equal("probably", fingerPrint["audioCodecs"]["aac"].Value<string>());
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