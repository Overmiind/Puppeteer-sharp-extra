using PuppeteerExtraSharpLite.Plugins.ExtraStealth.Evasions;

using PuppeteerSharp;

namespace PuppeteerExtraSharpLite.Plugins.ExtraStealth;

public class StealthPlugin : PuppeteerExtraPlugin, IOnPageCreatedPlugin {
    private readonly IPuppeteerExtraPluginOptions[] _options;

    public override string Name => nameof(StealthPlugin);

    //TODO: Are they really required or preferred by default?
    protected override string[] RequiredPlugins =>
    [
        nameof(WebDriver),
        nameof(ChromeSci),
        nameof(ChromeRuntime),
        nameof(Codec),
        nameof(Languages),
        nameof(OutDimensions),
        nameof(Permissions),
        nameof(UserAgent),
        nameof(Vendor),
        nameof(WebGl),
        nameof(PluginEvasion),
        nameof(StackTrace),
        nameof(HardwareConcurrency),
        nameof(ContentWindow)
    ];

    public StealthPlugin(params IPuppeteerExtraPluginOptions[] options) : base() {
        _options = options;
    }

    public static PuppeteerExtraPlugin[] GetStandardEvasions() =>
    [
        new WebDriver(),
        // new ChromeApp(),
        new ChromeSci(),
        new ChromeRuntime(),
        new Codec(),
        new Languages(),
        new OutDimensions(),
        new Permissions(),
        new UserAgent(),
        new Vendor(),
        new WebGl(),
        new PluginEvasion(),
        new StackTrace(),
        new HardwareConcurrency(),
        new ContentWindow(),
    ];

    public async Task OnPageCreated(IPage page) {
        // Load Utils script first before any evasion scripts run
        await page.EvaluateExpressionOnNewDocumentAsync(Scripts.Utils);

        // Now load all the evasion scripts that depend on utils
        //TODO: Replace logic
        // foreach (var evasion in _standardEvasions) {
        //     await evasion.OnPageCreated(page);
        // }
    }
}