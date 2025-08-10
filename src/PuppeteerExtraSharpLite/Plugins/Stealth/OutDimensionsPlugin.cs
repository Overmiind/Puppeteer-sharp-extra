using PuppeteerSharp;

namespace PuppeteerExtraSharpLite.Plugins.Stealth;

/// <summary>
/// Normalizes window.outer{Width,Height} values to better reflect real window metrics.
/// </summary>
public class OutDimensionsPlugin : PuppeteerPlugin, IOnPageCreatedPlugin {
    /// <inheritdoc />
    public override string Name => nameof(OutDimensionsPlugin);

    /// <inheritdoc />
    public async Task OnPageCreated(IPage page) {
        await page.EvaluateExpressionOnNewDocumentAsync(Scripts.Outdimensions).ConfigureAwait(false);
    }
}
