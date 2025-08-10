using PuppeteerSharp;

namespace PuppeteerSharpToolkit.Plugins;

/// <summary>
/// Applies fixes for window.contentWindow related checks to better mimic real browsers.
/// </summary>
/// <remarks>
/// Make sure ContentWindow is the last registered stealth plugin
/// </remarks>
public class ContentWindowPlugin : PuppeteerPlugin, IOnTargetCreatedPlugin {
    /// <inheritdoc />
    public override string Name => nameof(ContentWindowPlugin);

    /// <inheritdoc />
    public async Task OnTargetCreated(Target target) {
        if (target.Type == TargetType.Page) {
            var page = await target.PageAsync().ConfigureAwait(false);
            await Stealth.RegisterUtilsAsync(page);
            await page.EvaluateExpressionOnNewDocumentAsync(Scripts.ContentWindow).ConfigureAwait(false);
        }
    }
}
