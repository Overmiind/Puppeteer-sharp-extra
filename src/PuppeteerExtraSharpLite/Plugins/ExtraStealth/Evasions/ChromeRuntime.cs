using PuppeteerSharp;

namespace PuppeteerExtraSharpLite.Plugins.ExtraStealth.Evasions;

public class ChromeRuntime : PuppeteerExtraPlugin, IOnPageCreatedPlugin {
    public override string Name => nameof(ChromeRuntime);

    // StealthPlugin injects utils.js
    protected override string[] RequiredPlugins => [nameof(StealthPlugin)];

    public Task OnPageCreated(IPage page) {
        return page.EvaluateExpressionOnNewDocumentAsync(Scripts.Runtime);
    }
}