using PuppeteerSharp;

namespace PuppeteerExtraSharpLite.Plugins.Stealth;

/// <summary>
/// 
/// </summary>
/// <remarks>
/// Make sure ContentWindow is the last registered stealth plugin
/// </remarks>
public class ContentWindowPlugin : PuppeteerPlugin, IOnPageCreatedPlugin {
    public override string Name => nameof(ContentWindowPlugin);

    // StealthPlugin injects utils.js
    protected override string[] RequiredPlugins => [nameof(StealthPlugin)];

    public Task OnPageCreated(IPage page) {
        return page.EvaluateExpressionOnNewDocumentAsync(Scripts.ContentWindow);
    }
}