using PuppeteerSharp;

namespace PuppeteerExtraSharpLite.Plugins.Stealth;

/// <summary>
/// Provides compat patches for chrome.app and chrome APIs used by some detection scripts.
/// </summary>
public class ChromeSciPlugin : PuppeteerPlugin, IOnPageCreatedPlugin {
    /// <inheritdoc />
    public override string Name => nameof(ChromeSciPlugin);

    // StealthPlugin injects utils.js
    /// <inheritdoc />
    protected override string[] RequiredPlugins => [nameof(StealthPlugin)];

    /// <inheritdoc />
    public Task OnPageCreated(IPage page) {
        return page.EvaluateExpressionOnNewDocumentAsync(Scripts.SCI);
    }
}
