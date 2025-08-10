using PuppeteerSharp;

namespace PuppeteerSharpToolkit.Plugins;

/// <summary>
/// Provides compat patches for chrome.app and chrome APIs used by some detection scripts.
/// </summary>
public class ChromeSciPlugin : PuppeteerPlugin, IOnTargetCreatedPlugin {
    /// <inheritdoc />
    public override string Name => nameof(ChromeSciPlugin);

    /// <inheritdoc />
    public async Task OnTargetCreated(Target target) {
        if (target.Type == TargetType.Page) {
            var page = await target.PageAsync().ConfigureAwait(false);
            await Stealth.RegisterUtilsAsync(page);
            await page.EvaluateExpressionOnNewDocumentAsync(Scripts.SCI).ConfigureAwait(false);
        }
    }
}
