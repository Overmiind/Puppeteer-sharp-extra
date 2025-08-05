using System.Linq;

using PuppeteerExtraSharpLite.Plugins.ExtraStealth.Evasions;

using PuppeteerSharp;

namespace PuppeteerExtraSharpLite.Plugins.ExtraStealth;

public class StealthPlugin : PuppeteerExtraPlugin {
    private readonly IPuppeteerExtraPluginOptions[] _options;
    private readonly List<PuppeteerExtraPlugin> _standardEvasions;

    public override string Name => nameof(StealthPlugin);


    public StealthPlugin(params IPuppeteerExtraPluginOptions[] options) : base() {
        _options = options;
        _standardEvasions = GetStandardEvasions();
    }

    private List<PuppeteerExtraPlugin> GetStandardEvasions() {
        return new List<PuppeteerExtraPlugin>()
    {
            new WebDriver(),
            // new ChromeApp(),
            new ChromeSci(),
            new ChromeRuntime(),
            new Codec(),
            new Languages(GetOptionByType<StealthLanguagesOptions>()!),
            new OutDimensions(),
            new Permissions(),
            new UserAgent(),
            new Vendor(GetOptionByType<StealthVendorSettings>()!),
            new WebGl(GetOptionByType<StealthWebGLOptions>()!),
            new PluginEvasion(),
            new StackTrace(),
            new HardwareConcurrency(GetOptionByType<StealthHardwareConcurrencyOptions>()!),
            new ContentWindow(),
        };
    }

    public override ICollection<PuppeteerExtraPlugin> GetDependencies() => _standardEvasions;

    public override async Task OnPageCreated(IPage page) {
        // Load Utils script first before any evasion scripts run
        var utilsScript = Scripts.Utils.WithSourceUrl("Utils.js");
        await page.EvaluateExpressionOnNewDocumentAsync(utilsScript);

        // Now load all the evasion scripts that depend on utils
        foreach (var evasion in _standardEvasions) {
            await evasion.OnPageCreated(page);
        }
    }

    private T? GetOptionByType<T>() where T : IPuppeteerExtraPluginOptions {
        return _options.OfType<T>().FirstOrDefault();
    }

    public void RemoveEvasionByType<T>() where T : PuppeteerExtraPlugin {
        _standardEvasions.RemoveAll(ev => ev is T);
    }
}