using PuppeteerSharp;

namespace PuppeteerExtraSharpLite.Plugins.ExtraStealth.Evasions;

/// <summary>
/// 
/// </summary>
/// <remarks>
/// Make sure ContentWindow is the last registered stealth plugin
/// </remarks>
public class ContentWindow : PuppeteerExtraPlugin, IOnPageCreatedPlugin {
    public override string Name => nameof(ContentWindow);

    // StealthPlugin injects utils.js
    protected override string[] RequiredPlugins => [nameof(StealthPlugin)];

    public Task OnPageCreated(IPage page) {
        return page.EvaluateExpressionOnNewDocumentAsync(Scripts.ContentWindow);
    }
}