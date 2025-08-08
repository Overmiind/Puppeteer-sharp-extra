using PuppeteerSharp;

namespace PuppeteerExtraSharpLite.Plugins.Stealth;

/// <summary>
/// Patches navigator.plugins and related properties to resemble a real browser environment.
/// </summary>
public class EvasionPlugin : PuppeteerPlugin, IOnPageCreatedPlugin {
    /// <inheritdoc />
    public override string Name => nameof(EvasionPlugin);

    // StealthPlugin injects utils.js
    /// <inheritdoc />
    protected override string[] RequiredPlugins => [nameof(StealthPlugin)];

    /// <inheritdoc />
    public async Task OnPageCreated(IPage page) {
        await page.EvaluateExpressionOnNewDocumentAsync(Scripts.Plugin);
    }
}
