using PuppeteerSharp;

namespace PuppeteerExtraSharpLite.Plugins.Stealth;

public class HardwareConcurrencyPlugin : PuppeteerPlugin, IOnPageCreatedPlugin {
    public override string Name => nameof(HardwareConcurrencyPlugin);

    public readonly int ConcurrencyLevel;

    public HardwareConcurrencyPlugin(int concurrencyLevel = 4) : base() {
        ConcurrencyLevel = concurrencyLevel;
    }

    // StealthPlugin injects utils.js
    protected override string[] RequiredPlugins => [nameof(StealthPlugin)];

    public Task OnPageCreated(IPage page) {
        return page.EvaluateFunctionAsync(Scripts.HardwareConcurrency, [ConcurrencyLevel]);
    }
}