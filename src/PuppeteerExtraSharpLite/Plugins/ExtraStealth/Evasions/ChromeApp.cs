using PuppeteerSharp;

namespace PuppeteerExtraSharpLite.Plugins.ExtraStealth.Evasions;

public class ChromeApp : PuppeteerExtraPlugin, IOnPageCreatedPlugin {
    public override string Name => nameof(ChromeApp);

    // StealthPlugin injects utils.js
    protected override string[] RequiredPlugins => [nameof(StealthPlugin)];

    public Task OnPageCreated(IPage page) {
        return page.EvaluateExpressionOnNewDocumentAsync(Scripts.ChromeApp);
    }
}