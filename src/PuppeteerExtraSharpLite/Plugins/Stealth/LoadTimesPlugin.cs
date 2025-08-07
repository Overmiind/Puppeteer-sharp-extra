using PuppeteerSharp;

namespace PuppeteerExtraSharpLite.Plugins.Stealth;

public class LoadTimesPlugin : PuppeteerPlugin, IOnPageCreatedPlugin {
    public override string Name => nameof(LoadTimesPlugin);

    // StealthPlugin injects utils.js
    protected override string[] RequiredPlugins => [nameof(StealthPlugin)];

    public Task OnPageCreated(IPage page) {
        return page.EvaluateExpressionOnNewDocumentAsync(Scripts.LoadTimes);
    }
}