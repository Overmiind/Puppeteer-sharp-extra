using PuppeteerSharp;

namespace PuppeteerExtraSharpLite.Plugins.ExtraStealth.Evasions;

public class HardwareConcurrency : PuppeteerExtraPlugin, IOnPageCreatedPlugin {
    public override string Name => nameof(HardwareConcurrency);

    public StealthHardwareConcurrencyOptions Options { get; }

    public HardwareConcurrency(StealthHardwareConcurrencyOptions? options = null) : base() {
        Options = options ?? new StealthHardwareConcurrencyOptions(4);
    }

    public Task OnPageCreated(IPage page) {
        return page.EvaluateFunctionAsync(Scripts.HardwareConcurrency, [Options.Concurrency]);
    }
}

public class StealthHardwareConcurrencyOptions : IPuppeteerExtraPluginOptions {
    public int Concurrency { get; }

    public StealthHardwareConcurrencyOptions(int concurrency) {
        Concurrency = concurrency;
    }
}