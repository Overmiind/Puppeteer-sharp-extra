using PuppeteerSharp;

namespace PuppeteerExtraSharpLite.Plugins.Stealth;

/// <summary>
/// Provides a window.chrome.loadTimes shim for legacy scripts that check for it.
/// </summary>
public class LoadTimesPlugin : PuppeteerPlugin, IOnPageCreatedPlugin {
    /// <inheritdoc />
    public override string Name => nameof(LoadTimesPlugin);

    // StealthPlugin injects utils.js
    /// <inheritdoc />
    protected override string[] RequiredPlugins => [nameof(StealthPlugin)];

    /// <inheritdoc />
    public Task OnPageCreated(IPage page) {
        return page.EvaluateExpressionOnNewDocumentAsync(Scripts.LoadTimes);
    }
}
