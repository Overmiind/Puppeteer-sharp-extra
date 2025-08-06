using PuppeteerSharp;

namespace PuppeteerExtraSharpLite.Plugins.ExtraStealth.Evasions;

public class HardwareConcurrency : PuppeteerExtraPlugin, IOnPageCreatedPlugin {
    public override string Name => nameof(HardwareConcurrency);

    public readonly int ConcurrencyLevel;

    public HardwareConcurrency(int concurrencyLevel = 4) : base() {
        ConcurrencyLevel = concurrencyLevel;
    }

    // StealthPlugin injects utils.js
    protected override string[] RequiredPlugins => [nameof(StealthPlugin)];

    public Task OnPageCreated(IPage page) {
        return page.EvaluateFunctionAsync(Scripts.HardwareConcurrency, [ConcurrencyLevel]);
    }
}

public class StealthHardwareConcurrencyOptions : IPuppeteerExtraPluginOptions {
    public int Concurrency { get; }

    public StealthHardwareConcurrencyOptions(int concurrency) {
        Concurrency = concurrency;
    }
}