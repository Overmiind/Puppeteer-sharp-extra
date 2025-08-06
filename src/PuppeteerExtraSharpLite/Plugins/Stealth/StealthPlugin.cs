using PuppeteerSharp;

namespace PuppeteerExtraSharpLite.Plugins.Stealth;

public class StealthPlugin : PuppeteerExtraPlugin, IOnPageCreatedPlugin {
    public override string Name => nameof(StealthPlugin);

    public static PuppeteerExtraPlugin[] GetStandardEvasions() =>
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

    public async Task OnPageCreated(IPage page) {
        await page.EvaluateExpressionOnNewDocumentAsync(Scripts.Utils);
    }
}