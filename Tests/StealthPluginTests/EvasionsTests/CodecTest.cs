using System.Threading.Tasks;
using Extra.Tests.Utils;
using PuppeteerExtraSharp.Plugins.ExtraStealth;
using Xunit;

namespace Extra.Tests.StealthPluginTests.EvasionsTests
{
    public class CodecTest : BrowserDefault
    {
        [Fact]
        public async Task SupportsCodec()
        {
            var plugin = new StealthPlugin();
            var page = await LaunchAndGetPageAsync(plugin);
            await page.GoToAsync("https://google.com");
            var fingerPrint = await new FingerPrint().GetFingerPrint(page);

            var videoCodec = fingerPrint.GetProperty("videoCodecs");
            // Assert.Equal("probably", videoCodec.GetString("ogg"));
            Assert.Equal("probably", videoCodec.GetString("h264"));
            Assert.Equal("probably", videoCodec.GetString("webm"));

            var audioCodec = fingerPrint.GetProperty("audioCodecs");
            Assert.Equal("probably", audioCodec.GetString("ogg"));
            Assert.Equal("probably", audioCodec.GetString("mp3"));
            Assert.Equal("probably", audioCodec.GetString("wav"));
            Assert.Equal("maybe", audioCodec.GetString("m4a"));
            Assert.Equal("probably", audioCodec.GetString("aac"));
        }

        [Fact]
        public async Task NotLeakModifications()
        {
            var plugin = new StealthPlugin();
            var page = await LaunchAndGetPageAsync(plugin);

            var canPlay =
                await page.EvaluateFunctionAsync<string>(
                    "() => document.createElement('audio').canPlayType.toString()");
            Assert.Equal("function canPlayType() { [native code] }", canPlay);

            var canPlayHasName = await page.EvaluateFunctionAsync<string>(
                "() => document.createElement('audio').canPlayType.name");
            Assert.Equal("canPlayType", canPlayHasName);
        }
    }
}
