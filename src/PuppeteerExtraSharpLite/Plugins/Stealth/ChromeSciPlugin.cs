using PuppeteerSharp;

namespace PuppeteerExtraSharpLite.Plugins.Stealth;

public class ChromeSciPlugin : PuppeteerPlugin, IOnPageCreatedPlugin {
    public override string Name => nameof(ChromeSciPlugin);

    // StealthPlugin injects utils.js
    protected override string[] RequiredPlugins => [nameof(StealthPlugin)];

    public Task OnPageCreated(IPage page) {
        return page.EvaluateExpressionOnNewDocumentAsync(Scripts.SCI);
    }
}