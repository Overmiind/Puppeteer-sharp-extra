using System.Threading.Tasks;
using PuppeteerSharp;

namespace PuppeteerExtraSharp.Plugins.ExtraStealth.Evasions;

public class Vendor : PuppeteerExtraPlugin
{
    private readonly StealthVendorSettings _settings;

    public Vendor(StealthVendorSettings settings = null) : base("stealth-vendor")
    {
        _settings = settings ?? new StealthVendorSettings("Google Inc.");
    }

    public override async Task OnPageCreated(IPage page)
    {
        var script = Utils.GetScript("Vendor.js");
        await page.EvaluateFunctionOnNewDocumentAsync(script, _settings.Vendor);
    }
}

public class StealthVendorSettings : IPuppeteerExtraPluginOptions
{
    public string Vendor { get; }

    public StealthVendorSettings(string vendor)
    {
        Vendor = vendor;
    }
}
