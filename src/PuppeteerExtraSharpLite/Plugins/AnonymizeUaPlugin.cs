using System.Text.RegularExpressions;

using PuppeteerSharp;

namespace PuppeteerExtraSharpLite.Plugins;

public partial class AnonymizeUaPlugin : PuppeteerExtraPlugin, IOnPageCreatedPlugin {
    public override string Name => nameof(AnonymizeUaPlugin);

    public Func<string, string> UserAgentTransformer = static (s) => s;

    public async Task OnPageCreated(IPage page) {
        string ua = await page.Browser.GetUserAgentAsync();
        ua = ua.Replace("HeadlessChrome", "Chrome");

        ua = UserAgentRegex().Replace(ua, "(Windows NT 10.0; Win64; x64)");

        ua = UserAgentTransformer(ua);

        await page.SetUserAgentAsync(ua);
    }

    [GeneratedRegex(@"/\(([^)]+)\)/")]
    private static partial Regex UserAgentRegex();
}
