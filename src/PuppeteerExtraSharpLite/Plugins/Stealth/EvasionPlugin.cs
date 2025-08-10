using PuppeteerSharp;

namespace PuppeteerSharpToolkit.Plugins;

/// <summary>
/// Patches navigator.plugins and related properties to resemble a real browser environment.
/// </summary>
public class EvasionPlugin : PuppeteerPlugin, IOnTargetCreatedPlugin {
    /// <inheritdoc />
    public override string Name => nameof(EvasionPlugin);

    /// <inheritdoc />
    public async Task OnTargetCreated(Target target) {
        if (target.Type == TargetType.Page) {
            var page = await target.PageAsync().ConfigureAwait(false);
            await Stealth.RegisterUtilsAsync(page);
            await page.EvaluateExpressionOnNewDocumentAsync(Scripts.Evasion).ConfigureAwait(false);
        }
    }
}
