using PuppeteerSharp;

namespace PuppeteerExtraSharpLite.Plugins;

/// <summary>
/// Adjusts Permissions API behavior to reflect realistic states for HTTP/HTTPS origins.
/// </summary>
public class PermissionsPlugin : PuppeteerPlugin, IOnTargetCreatedPlugin {
    /// <inheritdoc />
    public override string Name => nameof(PermissionsPlugin);

    /// <inheritdoc />
    public async Task OnTargetCreated(Target target) {
        if (target.Type == TargetType.Page) {
            var page = await target.PageAsync().ConfigureAwait(false);
            await Stealth.RegisterUtilsAsync(page);
            await page.EvaluateExpressionOnNewDocumentAsync(Scripts.Permissions).ConfigureAwait(false);
        }
    }
}
