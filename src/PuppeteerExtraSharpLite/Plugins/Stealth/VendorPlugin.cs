using PuppeteerSharp;

namespace PuppeteerExtraSharpLite.Plugins.Stealth;

/// <summary>
/// Sets navigator.vendor to a desired value (defaults to "Google Inc.").
/// </summary>
public class VendorPlugin : PuppeteerPlugin, IOnTargetCreatedPlugin {
    /// <inheritdoc />
    public override string Name => nameof(VendorPlugin);

    // StealthPlugin injects utils.js
    /// <inheritdoc />
    protected override string[] RequiredPlugins => [nameof(StealthPlugin)];

    /// <summary>
    /// The vendor string to use for navigator.vendor.
    /// </summary>
    public readonly string Vendor;

    public VendorPlugin() : this("Google Inc.") { }

    public VendorPlugin(string vendor) {
        Vendor = vendor;
    }

    // /// <inheritdoc />
    // public async Task OnPageCreated(IPage page) {
    //     await page.EvaluateFunctionOnNewDocumentAsync(Scripts.Vendor, Vendor).ConfigureAwait(false);
    // }

    public async Task OnTargetCreated(Target target) {
        if (target.Type == TargetType.Page) {
            var page = await target.PageAsync().ConfigureAwait(false);
            await page.EvaluateFunctionOnNewDocumentAsync(Scripts.Vendor, Vendor).ConfigureAwait(false);
        }
    }
}