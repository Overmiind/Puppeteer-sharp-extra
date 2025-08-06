using PuppeteerExtraSharpLite.Plugins.ExtraStealth.Evasions;

using PuppeteerSharp;

namespace PuppeteerExtraSharpLite.Plugins.ExtraStealth;

public class StealthPlugin : PuppeteerExtraPlugin, IOnPageCreatedPlugin {
    public override string Name => nameof(StealthPlugin);

    public static PuppeteerExtraPlugin[] GetStandardEvasions() =>
    [
        new WebDriver(),
        new ChromeSci(),
        new ChromeRuntime(),
        new Codec(),
        new LanguagesPlugin(),
        new OutDimensions(),
        new Permissions(),
        new UserAgent(),
        new Vendor(),
        new WebGl(),
        new PluginEvasion(),
        new StackTrace(),
        new HardwareConcurrency(),
        new ContentWindow(), // Keep last
    ];

    public async Task OnPageCreated(IPage page) {
        await page.EvaluateExpressionOnNewDocumentAsync(Scripts.Utils);
    }
}