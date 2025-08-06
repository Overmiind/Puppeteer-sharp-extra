using PuppeteerExtraSharpLite.Tests.Utils;

using PuppeteerExtraSharpLite.Plugins.ExtraStealth.Evasions;
using PuppeteerExtraSharpLite.Plugins.ExtraStealth;

namespace PuppeteerExtraSharpLite.Tests.StealthPluginTests;

public partial class StealthPluginTests {
    [Fact]
    public async Task Codec_Plugin_SupportsCodec() {
        var pluginManager = new PluginManager();
        pluginManager.Register(new StealthPlugin()).Register(new Codec());

        await using var browser = await pluginManager.LaunchAsync();
        using var page = await browser.NewPageAsync();

        await page.GoToAsync("https://google.com");
        var fingerPrint = await page.GetFingerPrint();

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
    public async Task Codec_Plugin_ShouldNot_LeakModifications() {
        var pluginManager = new PluginManager();
        pluginManager.Register(new StealthPlugin()).Register(new Codec());

        using var browser = await pluginManager.LaunchAsync();
        using var page = await browser.NewPageAsync();

        var canPlay =
            await page.EvaluateFunctionAsync<string>(
                "() => document.createElement('audio').canPlayType.toString()");
        Assert.Equal("function canPlayType() { [native code] }", canPlay);

        var canPlayHasName = await page.EvaluateFunctionAsync<string>(
            "() => document.createElement('audio').canPlayType.name");
        Assert.Equal("canPlayType", canPlayHasName);
    }
}