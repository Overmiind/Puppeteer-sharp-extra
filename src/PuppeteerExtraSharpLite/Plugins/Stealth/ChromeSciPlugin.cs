using PuppeteerSharp;

namespace PuppeteerExtraSharpLite.Plugins.Stealth;

/// <summary>
/// Provides compat patches for chrome.app and chrome APIs used by some detection scripts.
/// </summary>
public class ChromeSciPlugin : PuppeteerPlugin, IOnTargetCreatedPlugin {
    /// <inheritdoc />
    public override string Name => nameof(ChromeSciPlugin);

    // StealthPlugin injects utils.js
    /// <inheritdoc />
    protected override string[] RequiredPlugins => [nameof(StealthPlugin)];

    // /// <inheritdoc />
    // public async Task OnPageCreated(IPage page) {
    //     await page.EvaluateExpressionOnNewDocumentAsync(Scripts.SCI).ConfigureAwait(false);
    // }

    public async Task OnTargetCreated(Target target) {
        if (target.Type == TargetType.Page) {
            var page = await target.PageAsync().ConfigureAwait(false);
            await page.EvaluateExpressionOnNewDocumentAsync(Scripts.SCI).ConfigureAwait(false);
        }
    }
}
