using PuppeteerSharp;

namespace PuppeteerExtraSharpLite.Plugins.Stealth;

/// <summary>
/// Overrides navigator.hardwareConcurrency to a specified logical CPU count.
/// </summary>
public class HardwareConcurrencyPlugin : PuppeteerPlugin, IOnPageCreatedPlugin {
    /// <inheritdoc />
    public override string Name => nameof(HardwareConcurrencyPlugin);

    /// <summary>
    /// The logical CPU count to expose via navigator.hardwareConcurrency.
    /// </summary>
    public readonly int ConcurrencyLevel;

    public HardwareConcurrencyPlugin() : this(4) { }

    public HardwareConcurrencyPlugin(int concurrencyLevel) {
        ConcurrencyLevel = concurrencyLevel;
    }

    // StealthPlugin injects utils.js
    /// <inheritdoc />
    protected override string[] RequiredPlugins => [nameof(StealthPlugin)];

    /// <inheritdoc />
    public Task OnPageCreated(IPage page) {
        return page.EvaluateFunctionAsync(Scripts.HardwareConcurrency, [ConcurrencyLevel]);
    }
}
