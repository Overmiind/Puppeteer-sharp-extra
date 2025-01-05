using System.Threading.Tasks;
using Extra.Tests.Utils;
using Newtonsoft.Json.Linq;
using PuppeteerExtraSharp.Plugins.ExtraStealth.Evasions;
using Xunit;

namespace Extra.Tests.StealthPluginTests.EvasionsTests;

public class CodecTest : BrowserDefault
{
    [Fact]
    public async Task SupportsCodec()
    {
        var plugin = new Codec();
        var page = await LaunchAndGetPage(plugin);
        await page.GoToAsync("https://google.com");
        var fingerPrint = await new FingerPrint().GetFingerPrint(page);

        Assert.Equal("", fingerPrint["videoCodecs"]["ogg"].GetValue<string>());
        Assert.Equal("probably", fingerPrint["videoCodecs"]["h264"].GetValue<string>());
        Assert.Equal("probably", fingerPrint["videoCodecs"]["webm"].GetValue<string>());

        Assert.Equal("probably", fingerPrint["audioCodecs"]["ogg"].GetValue<string>());
        Assert.Equal("probably", fingerPrint["audioCodecs"]["mp3"].GetValue<string>());
        Assert.Equal("probably", fingerPrint["audioCodecs"]["wav"].GetValue<string>());
        Assert.Equal("maybe", fingerPrint["audioCodecs"]["m4a"].GetValue<string>());
        Assert.Equal("probably", fingerPrint["audioCodecs"]["aac"].GetValue<string>());
    }

    [Fact]
    public async Task NotLeakModifications()
    {
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
