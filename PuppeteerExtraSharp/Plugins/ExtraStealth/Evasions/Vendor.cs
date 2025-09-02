using System.Threading.Tasks;
using PuppeteerSharp;

namespace PuppeteerExtraSharp.Plugins.ExtraStealth.Evasions;

public class Vendor(StealthVendorSettings settings = null) : PuppeteerExtraPlugin("stealth-vendor")
{
    private readonly StealthVendorSettings _settings = settings ?? new StealthVendorSettings("Google Inc.");

    protected internal override async Task OnPageCreatedAsync(IPage page)
    {
        var script = StealthUtils.GetScript("Vendor.js");
        await page.EvaluateFunctionOnNewDocumentAsync(script, _settings.Vendor);
    }
}

public class StealthVendorSettings(string vendor) : IPuppeteerExtraPluginOptions
{
    public string Vendor { get; } = vendor;
}