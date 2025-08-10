using PuppeteerSharp;

namespace PuppeteerSharpToolkit.Plugins;

/// <summary>
/// Normalizes audio and video codec support to avoid revealing headless characteristics.
/// </summary>
public class CodecPlugin : PuppeteerPlugin, IOnTargetCreatedPlugin {
    /// <inheritdoc />
    public override string Name => nameof(CodecPlugin);

    /// <inheritdoc />
    public async Task OnTargetCreated(Target target) {
        if (target.Type == TargetType.Page) {
            var page = await target.PageAsync().ConfigureAwait(false);
            await Stealth.RegisterUtilsAsync(page);
            await page.EvaluateExpressionOnNewDocumentAsync(Scripts.Codec).ConfigureAwait(false);
        }
    }
}
