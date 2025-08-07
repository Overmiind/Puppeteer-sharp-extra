using PuppeteerSharp;

namespace PuppeteerExtraSharpLite.Plugins.Stealth;

public class VendorPlugin : PuppeteerPlugin, IOnPageCreatedPlugin {
    public override string Name => nameof(VendorPlugin);

    public readonly string Vendor;

    public VendorPlugin(string vendor = "") {
        Vendor = vendor.Length > 0
                ? vendor
                : "Google Inc.";
    }

    // StealthPlugin injects utils.js
    protected override string[] RequiredPlugins => [nameof(StealthPlugin)];

    public async Task OnPageCreated(IPage page) {
        await page.EvaluateFunctionOnNewDocumentAsync(Scripts.Vendor, [Vendor]);
    }
}
