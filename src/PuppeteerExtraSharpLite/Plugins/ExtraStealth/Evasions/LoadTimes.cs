using PuppeteerSharp;

namespace PuppeteerExtraSharpLite.Plugins.ExtraStealth.Evasions;

public class LoadTimes : PuppeteerExtraPlugin, IOnPageCreatedPlugin {
    public override string Name => nameof(LoadTimes);

    // StealthPlugin injects utils.js
    protected override string[] RequiredPlugins => [nameof(StealthPlugin)];

    public Task OnPageCreated(IPage page) {
        return page.EvaluateExpressionOnNewDocumentAsync(Scripts.LoadTimes);
    }
}