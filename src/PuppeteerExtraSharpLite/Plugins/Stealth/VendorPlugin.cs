using PuppeteerSharp;

namespace PuppeteerExtraSharpLite.Plugins.Stealth;

/// <summary>
/// Sets navigator.vendor to a desired value (defaults to "Google Inc.").
/// </summary>
public class VendorPlugin : PuppeteerPlugin, IOnPageCreatedPlugin {
    /// <inheritdoc />
    public override string Name => nameof(VendorPlugin);

    /// <summary>
    /// The vendor string to use for navigator.vendor.
    /// </summary>
    public readonly string Vendor;

    public VendorPlugin(string vendor = "") {
        Vendor = vendor.Length > 0
                ? vendor
                : "Google Inc.";
    }

    // StealthPlugin injects utils.js
    /// <inheritdoc />
    protected override string[] RequiredPlugins => [nameof(StealthPlugin)];

    /// <inheritdoc />
    public async Task OnPageCreated(IPage page) {
        await page.EvaluateFunctionOnNewDocumentAsync(Scripts.Vendor, [Vendor]);
    }
}
