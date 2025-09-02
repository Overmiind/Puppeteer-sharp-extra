using System.Threading.Tasks;
using PuppeteerSharp;

namespace PuppeteerExtraSharp.Plugins.ExtraStealth.Evasions;

public class HardwareConcurrency(StealthHardwareConcurrencyOptions options = null)
    : PuppeteerExtraPlugin("stealth/hardwareConcurrency")
{
    public StealthHardwareConcurrencyOptions Options { get; } = options ?? new StealthHardwareConcurrencyOptions(4);

    protected internal override Task OnPageCreatedAsync(IPage page)
    {
        var script = StealthUtils.GetScript("HardwareConcurrency.js");
        return StealthUtils.EvaluateOnNewPage(page, script, Options.Concurrency);
    }
}

public class StealthHardwareConcurrencyOptions(int concurrency) : IPuppeteerExtraPluginOptions
{
    public int Concurrency { get; } = concurrency;
}