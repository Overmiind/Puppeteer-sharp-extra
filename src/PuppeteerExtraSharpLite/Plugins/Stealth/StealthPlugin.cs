using PuppeteerSharp;

namespace PuppeteerExtraSharpLite.Plugins.Stealth;

/// <summary>
/// Aggregates common stealth evasions and utilities. This plugin is typically registered first
/// so other stealth plugins can rely on the injected helper scripts.
/// </summary>
public class StealthPlugin : PuppeteerPlugin, IOnPageCreatedPlugin {
    /// <inheritdoc />
    public override string Name => nameof(StealthPlugin);

    /// <summary>
    /// Gets the standard set of stealth evasions recommended for most scenarios.
    /// </summary>
    public static PuppeteerPlugin[] GetStandardEvasions() =>
    [
        new WebDriverPlugin(),
        new ChromeSciPlugin(),
        new ChromeRuntimePlugin(),
        new CodecPlugin(),
        new LanguagesPlugin(),
        new OutDimensionsPlugin(),
        new PermissionsPlugin(),
        new UserAgentPlugin(),
        new VendorPlugin(),
        new WebGlPlugin(),
        new EvasionPlugin(),
        new StackTracePlugin(),
        new HardwareConcurrencyPlugin(),
        new ContentWindowPlugin(), // Keep last
    ];

    /// <inheritdoc />
    public async Task OnPageCreated(IPage page) {
        await page.EvaluateExpressionOnNewDocumentAsync(Scripts.Utils);
    }
}
