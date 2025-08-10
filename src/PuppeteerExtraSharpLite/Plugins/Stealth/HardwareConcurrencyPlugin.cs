using PuppeteerSharp;

namespace PuppeteerExtraSharpLite.Plugins.Stealth;

/// <summary>
/// Overrides navigator.hardwareConcurrency to a specified logical CPU count.
/// </summary>
public class HardwareConcurrencyPlugin : PuppeteerPlugin, IOnTargetCreatedPlugin {
    /// <inheritdoc />
    public override string Name => nameof(HardwareConcurrencyPlugin);

    // StealthPlugin injects utils.js
    /// <inheritdoc />
    protected override string[] RequiredPlugins => [nameof(StealthPlugin)];

    /// <summary>
    /// The logical CPU count to expose via navigator.hardwareConcurrency.
    /// </summary>
    public readonly int ConcurrencyLevel;

    public HardwareConcurrencyPlugin() : this(4) { }

    public HardwareConcurrencyPlugin(int concurrencyLevel) {
        ConcurrencyLevel = concurrencyLevel;
    }

    // /// <inheritdoc />
    // public async Task OnPageCreated(IPage page) {
    //     await page.EvaluateFunctionAsync(Scripts.HardwareConcurrency, ConcurrencyLevel).ConfigureAwait(false);
    // }

    public async Task OnTargetCreated(Target target) {
        if (target.Type == TargetType.Page) {
            var page = await target.PageAsync().ConfigureAwait(false);
            await page.EvaluateFunctionAsync(Scripts.HardwareConcurrency, ConcurrencyLevel).ConfigureAwait(false);
        }
    }
}
