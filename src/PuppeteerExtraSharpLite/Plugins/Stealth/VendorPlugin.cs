using PuppeteerSharp;

namespace PuppeteerExtraSharpLite.Plugins;

/// <summary>
/// Sets navigator.vendor to a desired value (defaults to "Google Inc.").
/// </summary>
public class VendorPlugin : PuppeteerPlugin, IOnTargetCreatedPlugin {
    /// <inheritdoc />
    public override string Name => nameof(VendorPlugin);

    /// <summary>
    /// The vendor string to use for navigator.vendor.
    /// </summary>
    public readonly string Vendor;

    /// <summary>
    /// Sets navigator.vendor to a "Google Inc.".
    /// </summary>
    public VendorPlugin() : this("Google Inc.") { }

    /// <summary>
    /// Sets navigator.vendor to a desired <paramref name="vendor"/>
    /// </summary>
    public VendorPlugin(string vendor) {
        Vendor = vendor;
    }

    /// <inheritdoc />
    public async Task OnTargetCreated(Target target) {
        if (target.Type == TargetType.Page) {
            var page = await target.PageAsync().ConfigureAwait(false);
            await Stealth.RegisterUtilsAsync(page);
            await page.EvaluateFunctionOnNewDocumentAsync(Scripts.Vendor, Vendor).ConfigureAwait(false);
        }
    }
}