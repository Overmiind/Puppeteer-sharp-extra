using PuppeteerSharp;

namespace PuppeteerExtraSharpLite.Plugins.ExtraStealth.Evasions;

public class HardwareConcurrency : PuppeteerExtraPlugin {
    public StealthHardwareConcurrencyOptions Options { get; }

    public HardwareConcurrency(StealthHardwareConcurrencyOptions? options = null) : base("stealth/hardwareConcurrency") {
        Options = options ?? new StealthHardwareConcurrencyOptions(4);
    }

    public override Task OnPageCreated(IPage page) {
        var script = Scripts.HardwareConcurrency.WithSourceUrl("HardwareConcurrency.js");
        return Utils.EvaluateOnNewPage(page, script, Options.Concurrency);
    }
}

public class StealthHardwareConcurrencyOptions : IPuppeteerExtraPluginOptions {
    public int Concurrency { get; }

    public StealthHardwareConcurrencyOptions(int concurrency) {
        Concurrency = concurrency;
    }
}