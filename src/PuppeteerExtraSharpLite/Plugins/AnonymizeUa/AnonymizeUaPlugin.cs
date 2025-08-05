using System.Text.RegularExpressions;

using PuppeteerSharp;

namespace PuppeteerExtraSharpLite.Plugins.AnonymizeUa;

public partial class AnonymizeUaPlugin : PuppeteerExtraPlugin {
    public override string Name => nameof(AnonymizeUaPlugin);

    public AnonymizeUaPlugin() : base() {
    }

    private Func<string, string>? _customAction;

    public void CustomizeUa(Func<string, string> uaAction) {
        _customAction = uaAction;
    }

    public override async Task OnPageCreated(IPage page) {
        string ua = await page.Browser.GetUserAgentAsync();
        ua = ua.Replace("HeadlessChrome", "Chrome");

        ua = UserAgentRegex().Replace(ua, "(Windows NT 10.0; Win64; x64)");

        if (_customAction != null) {
            ua = _customAction(ua);
        }

        await page.SetUserAgentAsync(ua);
    }

    [GeneratedRegex(@"/\(([^)]+)\)/")]
    private static partial Regex UserAgentRegex();
}
