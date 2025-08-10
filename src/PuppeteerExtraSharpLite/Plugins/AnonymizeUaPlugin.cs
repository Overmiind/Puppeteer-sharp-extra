using System.Text.RegularExpressions;

using PuppeteerSharp;

namespace PuppeteerExtraSharpLite.Plugins;

public partial class AnonymizeUaPlugin : PuppeteerPlugin, IOnTargetCreatedPlugin {
    public override string Name => nameof(AnonymizeUaPlugin);

    public Func<string, string> UserAgentTransformer { get; set; } = static s => s;

    // public async Task OnPageCreated(IPage page) {
    //     string ua = await page.Browser.GetUserAgentAsync().ConfigureAwait(false);
    //     ua = ua.Replace("HeadlessChrome", "Chrome");

    //     ua = UserAgentRegex().Replace(ua, "(Windows NT 10.0; Win64; x64)");

    //     ua = UserAgentTransformer(ua);

    //     await page.SetUserAgentAsync(ua).ConfigureAwait(false);
    // }

    [GeneratedRegex(@"/\(([^)]+)\)/")]
    private static partial Regex UserAgentRegex();

    public async Task OnTargetCreated(Target target) {
        if (target.Type == TargetType.Page) {
            var page = await target.PageAsync().ConfigureAwait(false);
            string ua = await page.Browser.GetUserAgentAsync().ConfigureAwait(false);
            ua = ua.Replace("HeadlessChrome", "Chrome");

            ua = UserAgentRegex().Replace(ua, "(Windows NT 10.0; Win64; x64)");

            ua = UserAgentTransformer(ua);

            await page.SetUserAgentAsync(ua).ConfigureAwait(false);
        }
    }
}
