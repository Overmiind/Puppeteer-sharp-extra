using PuppeteerSharp;

namespace PuppeteerExtraSharpLite.Plugins;

/// <summary>
/// Overrides navigator.hardwareConcurrency to a specified logical CPU count.
/// </summary>
public class HardwareConcurrencyPlugin : PuppeteerPlugin, IOnTargetCreatedPlugin {
    /// <inheritdoc />
    public override string Name => nameof(HardwareConcurrencyPlugin);

    /// <summary>
    /// The logical CPU count to expose via navigator.hardwareConcurrency.
    /// </summary>
    public readonly int ConcurrencyLevel;

    /// <summary>
    /// Overrides navigator.hardwareConcurrency with 4 logical CPU count.
    /// </summary>
    public HardwareConcurrencyPlugin() : this(4) { }

    /// <summary>
    /// Overrides navigator.hardwareConcurrency with <paramref name="concurrencyLevel"/>.
    /// </summary>
    public HardwareConcurrencyPlugin(int concurrencyLevel) {
        ConcurrencyLevel = concurrencyLevel;
    }

    /// <inheritdoc />
    public async Task OnTargetCreated(Target target) {
        if (target.Type == TargetType.Page) {
            var page = await target.PageAsync().ConfigureAwait(false);
            await Stealth.RegisterUtilsAsync(page);
            await page.EvaluateFunctionAsync(Scripts.HardwareConcurrency, ConcurrencyLevel).ConfigureAwait(false);
        }
    }
}
