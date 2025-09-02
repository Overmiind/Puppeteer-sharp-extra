using System.Text.Json;
using System.Threading.Tasks;
using Extra.Tests.Utils;
using PuppeteerExtraSharp.Plugins.ExtraStealth;
using Xunit;

namespace Extra.Tests.StealthPluginTests.EvasionsTests
{
    public class PermissionsTest : BrowserDefault
    {
        [Fact]
        public async Task ShouldBeDenied()
        {
            var plugin = new StealthPlugin();
            var page = await LaunchAndGetPageAsync();

            var notificationPermission = await page.EvaluateFunctionAsync<JsonElement>(GetNotificationPermissionScript);
            Assert.Equal("denied", notificationPermission.GetString("state"));
            Assert.Equal("denied", notificationPermission.GetString("permission"));

            await page.GoToAsync("https://example.com");

            notificationPermission = await page.EvaluateFunctionAsync<JsonElement>(GetNotificationPermissionScript);
            Assert.Equal("prompt", notificationPermission.GetString("state"));
            Assert.Equal("default", notificationPermission.GetString("permission"));
        }

        private const string GetNotificationPermissionScript =
            @"async () => {
  const { state, onchange } = await navigator.permissions.query({
    name: 'notifications'
  })
  return {
    state,
    onchange,
    permission: Notification.permission
  }
}";
    }
}