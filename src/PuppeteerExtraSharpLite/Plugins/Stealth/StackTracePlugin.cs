using PuppeteerSharp;

namespace PuppeteerExtraSharpLite.Plugins.Stealth;

/// <summary>
/// Adjusts Error.stack behavior to better match native browser implementations.
/// </summary>
public class StackTracePlugin : PuppeteerPlugin, IOnTargetCreatedPlugin {
    /// <inheritdoc />
    public override string Name => nameof(StackTracePlugin);

    // /// <inheritdoc />
    // public async Task OnPageCreated(IPage page) {
    //     await page.EvaluateExpressionOnNewDocumentAsync(Scripts.Stacktrace).ConfigureAwait(false);
    // }

    public async Task OnTargetCreated(Target target) {
        if (target.Type == TargetType.Page) {
            var page = await target.PageAsync().ConfigureAwait(false);
            await page.EvaluateExpressionOnNewDocumentAsync(Scripts.Stacktrace).ConfigureAwait(false);
        }
    }
}
