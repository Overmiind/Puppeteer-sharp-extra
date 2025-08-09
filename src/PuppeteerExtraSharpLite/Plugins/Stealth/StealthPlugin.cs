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
    /// Contracts for creating the standard evasion plugins
    /// </summary>
    /// <returns></returns>
    private static readonly PluginContract[] StandardEvasionsContracts =
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
        // Initialize list with 0 capacity to avoid allocation since expected unrelated plugins is 0
        List<PuppeteerPlugin> unrelated = new(0);
        // Add unrelated plugins to list
        foreach (var plugin in pluginOverride) {
            bool isRelated = false;
            foreach (var s in standardEvasions) {
                if (plugin.Name == s.PluginName) {
                    isRelated = true;
                }
            }
            if (!isRelated) {
                unrelated.Add(plugin);
            }
        }
        // create array with enough capacity for unrelated plugins
        var outputs = new PuppeteerPlugin[standardEvasions.Length + unrelated.Count];
        for (int i = 0; i < standardEvasions.Length; i++) {
            var currentEvasion = standardEvasions[i];
            bool overridden = false;
            // override exists - set using override & break
            foreach (var plugin in pluginOverride) {
                if (currentEvasion.PluginName == plugin.Name) {
                    outputs[i] = plugin;
                    overridden = true;
                    break;
                }
            }
            // no override - use default factory
            if (!overridden) {
                outputs[i] = currentEvasion.Factory();
            }
        }
        if (unrelated.Count > 0) {
            unrelated.CopyTo(outputs.AsSpan(standardEvasions.Length));
        }
        return outputs;
    }

    /// <inheritdoc />
    public async Task OnPageCreated(IPage page) {
        await page.EvaluateExpressionOnNewDocumentAsync(Scripts.Utils);
    }

    private readonly record struct PluginContract(string PluginName, Func<PuppeteerPlugin> Factory);
}
