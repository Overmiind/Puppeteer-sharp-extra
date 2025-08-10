using PuppeteerSharp;

namespace PuppeteerExtraSharpLite.Plugins.Stealth;

/// <summary>
/// Patches navigator.plugins and related properties to resemble a real browser environment.
/// </summary>
public class EvasionPlugin : PuppeteerPlugin, IOnTargetCreatedPlugin {
    /// <inheritdoc />
    public override string Name => nameof(EvasionPlugin);

    // StealthPlugin injects utils.js
    /// <inheritdoc />
    protected override string[] RequiredPlugins => [nameof(StealthPlugin)];

    // /// <inheritdoc />
    // public async Task OnPageCreated(IPage page) {
    //     await page.EvaluateExpressionOnNewDocumentAsync(Scripts.Plugin).ConfigureAwait(false);
    // }

    public async Task OnTargetCreated(Target target) {
        if (target.Type == TargetType.Page) {
            var page = await target.PageAsync().ConfigureAwait(false);
            await page.EvaluateExpressionOnNewDocumentAsync(Scripts.Plugin).ConfigureAwait(false);
        }
    }
}
