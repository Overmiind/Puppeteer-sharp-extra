using PuppeteerSharp;

namespace PuppeteerExtraSharpLite.Plugins.Stealth;

public class ChromeRuntimePlugin : PuppeteerExtraPlugin, IOnPageCreatedPlugin {
    public override string Name => nameof(ChromeRuntimePlugin);

    // StealthPlugin injects utils.js
    protected override string[] RequiredPlugins => [nameof(StealthPlugin)];

    public Task OnPageCreated(IPage page) {
        return page.EvaluateExpressionOnNewDocumentAsync(Scripts.Runtime);
    }
}