using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Extra.Tests.Utils;
using PuppeteerExtraSharp.Plugins.ExtraStealth.Evasions;
using Xunit;

namespace Extra.Tests.StealthPluginTests
{
    public class PermissionsTest: BrowserDefault
    {
        [Fact]
        public async Task ShouldBeDenied()
        {
            var plugin = new Permissions();
            var page = await LaunchAndGetPage(plugin);
            await page.GoToAsync("https://google.com");

            var finger = await new FingerPrint().GetFingerPrint(page);

            Assert.Equal("denied", finger["permissions"]["state"]);
        }
    }
}
