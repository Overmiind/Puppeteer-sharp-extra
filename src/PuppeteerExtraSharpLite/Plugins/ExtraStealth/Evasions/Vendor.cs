using PuppeteerSharp;

namespace PuppeteerExtraSharpLite.Plugins.ExtraStealth.Evasions;

public class Vendor : PuppeteerExtraPlugin, IOnPageCreatedPlugin {
    public override string Name => nameof(Vendor);

    private readonly StealthVendorSettings _settings;

    public Vendor(StealthVendorSettings? settings = null) : base() {
        _settings = settings ?? new StealthVendorSettings("Google Inc.");
    }

    // StealthPlugin injects utils.js
    protected override string[] RequiredPlugins => [nameof(StealthPlugin)];

    public async Task OnPageCreated(IPage page) {
        await page.EvaluateFunctionOnNewDocumentAsync(Scripts.Vendor, _settings.Vendor);
    }
}

public class StealthVendorSettings : IPuppeteerExtraPluginOptions {
    public string Vendor { get; }

    public StealthVendorSettings(string vendor) {
        Vendor = vendor;
    }
}
