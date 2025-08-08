using PuppeteerSharp;

namespace PuppeteerExtraSharpLite.Plugins.Stealth;

/// <summary>
/// Applies fixes for window.contentWindow related checks to better mimic real browsers.
/// </summary>
/// <remarks>
/// Make sure ContentWindow is the last registered stealth plugin
/// </remarks>
public class ContentWindowPlugin : PuppeteerPlugin, IOnPageCreatedPlugin {
    /// <inheritdoc />
    public override string Name => nameof(ContentWindowPlugin);

    // StealthPlugin injects utils.js
    /// <inheritdoc />
    protected override string[] RequiredPlugins => [nameof(StealthPlugin)];

    /// <inheritdoc />
    public Task OnPageCreated(IPage page) {
        return page.EvaluateExpressionOnNewDocumentAsync(Scripts.ContentWindow);
    }
}
