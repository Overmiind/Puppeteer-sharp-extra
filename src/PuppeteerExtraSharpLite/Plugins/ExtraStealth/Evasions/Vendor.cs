using PuppeteerSharp;

namespace PuppeteerExtraSharpLite.Plugins.ExtraStealth.Evasions;

public class Vendor : PuppeteerExtraPlugin {
    public override string Name => nameof(Vendor);

    private readonly StealthVendorSettings _settings;

    public Vendor(StealthVendorSettings? settings = null) : base() {
        _settings = settings ?? new StealthVendorSettings("Google Inc.");
    }

    public override async Task OnPageCreated(IPage page) {
        var script = Scripts.Vendor.WithSourceUrl("Vendor.js");
        await page.EvaluateFunctionOnNewDocumentAsync(script, _settings.Vendor);
    }
}

public class StealthVendorSettings : IPuppeteerExtraPluginOptions {
    public string Vendor { get; }

    public StealthVendorSettings(string vendor) {
        Vendor = vendor;
    }
}
