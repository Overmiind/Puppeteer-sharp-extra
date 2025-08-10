using PuppeteerSharp;

namespace PuppeteerExtraSharpLite.Plugins.Stealth;

/// <summary>
/// Aggregates common stealth evasions and utilities. This plugin is typically registered first
/// so other stealth plugins can rely on the injected helper scripts.
/// </summary>
public class StealthPlugin : PuppeteerPlugin, IOnTargetCreatedPlugin {
    /// <inheritdoc />
    public override string Name => nameof(StealthPlugin);

    /// <summary>
    /// Contracts for creating the standard evasion plugins
    /// </summary>
    /// <returns></returns>
    internal static readonly PluginContract[] StandardEvasionsContracts =
    [
        new(nameof(WebDriverPlugin), () => new WebDriverPlugin()),
        new(nameof(ChromeSciPlugin), () => new ChromeSciPlugin()),
        new(nameof(ChromeRuntimePlugin), () => new ChromeRuntimePlugin()),
        new(nameof(CodecPlugin), () => new CodecPlugin()),
        new(nameof(LanguagesPlugin), () => new LanguagesPlugin()),
        new(nameof(OutDimensionsPlugin), () => new OutDimensionsPlugin()),
        new(nameof(PermissionsPlugin), () => new PermissionsPlugin()),
        new(nameof(UserAgentPlugin), () => new UserAgentPlugin()),
        new(nameof(VendorPlugin), () => new VendorPlugin()),
        new(nameof(WebGlPlugin), () => new WebGlPlugin()),
        new(nameof(EvasionPlugin), () => new EvasionPlugin()),
        new(nameof(StackTracePlugin), () => new StackTracePlugin()),
        new(nameof(HardwareConcurrencyPlugin), () => new HardwareConcurrencyPlugin()),
        new(nameof(ContentWindowPlugin), () => new ContentWindowPlugin())
    ];

    /// <summary>
    /// Gets the standard set of stealth evasions recommended for most scenarios. while supporting overriding a specific plugin
    /// </summary>
    public static PuppeteerPlugin[] GetStandardEvasions(params PuppeteerPlugin[] pluginOverride) {
        var standardEvasions = StandardEvasionsContracts;
        Span<bool> relationMap = stackalloc bool[pluginOverride.Length];
        int unrelated = 0;
        // Add unrelated plugins to list
        for (int i = 0; i < pluginOverride.Length; i++) {
            var plugin = pluginOverride[i];
            foreach (var s in standardEvasions) {
                if (plugin.Name == s.PluginName) {
                    relationMap[i] = true;
                    break;
                }
            }
            if (!relationMap[i]) {
                unrelated++;
            }
        }
        // create array with enough capacity for unrelated plugins
        var outputs = new PuppeteerPlugin[standardEvasions.Length + unrelated];
        for (int i = 0; i < standardEvasions.Length; i++) {
            var currentEvasion = standardEvasions[i];
            bool overridden = false;
            // override exists - set using override & break
            for (int j = 0; j < relationMap.Length; j++) {
                if (relationMap[j] && currentEvasion.PluginName == pluginOverride[j].Name) {
                    outputs[i] = pluginOverride[j];
                    overridden = true;
                    break;
                }
            }
            // no override - use default factory
            if (!overridden) {
                outputs[i] = currentEvasion.Factory();
            }
        }
        // Add unrelated
        if (unrelated > 0) {
            var extraSpan = outputs.AsSpan(standardEvasions.Length);
            int extraIndex = 0;
            for (int i = 0; i < relationMap.Length; i++) {
                if (!relationMap[i]) {
                    extraSpan[extraIndex++] = pluginOverride[i];
                }
            }
        }
        return outputs;
    }

    // /// <inheritdoc />
    // public async Task OnPageCreated(IPage page) {
    //     await page.EvaluateExpressionOnNewDocumentAsync(Scripts.Utils).ConfigureAwait(false);
    // }

    public async Task OnTargetCreated(Target target) {
        if (target.Type == TargetType.Page) {
            var page = await target.PageAsync().ConfigureAwait(false);
            await page.EvaluateExpressionOnNewDocumentAsync(Scripts.Utils).ConfigureAwait(false);
        }
    }

    internal readonly record struct PluginContract(string PluginName, Func<PuppeteerPlugin> Factory);
}
