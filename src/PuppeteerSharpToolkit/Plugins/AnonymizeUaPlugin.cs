using System.Text.RegularExpressions;

using PuppeteerSharp;

namespace PuppeteerSharpToolkit.Plugins;

/// <summary>
/// This plugin transforms the page's user agent upon navigation
/// </summary>
public partial class AnonymizeUaPlugin : PuppeteerPlugin, IOnTargetCreatedPlugin {
    /// <inheritdoc />
    public override string Name => nameof(AnonymizeUaPlugin);

    /// <summary>
    /// A user agent transformation function
    /// </summary>
    public Func<string, string> UserAgentTransformer { get; set; } = static s => s;

    [GeneratedRegex(@"/\(([^)]+)\)/")]
    private static partial Regex UserAgentRegex();

    /// <inheritdoc />
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
