using PuppeteerSharp;

namespace PuppeteerExtraSharpLite.Plugins.Stealth;

public class EvasionPlugin : PuppeteerExtraPlugin, IOnPageCreatedPlugin {
    public override string Name => nameof(EvasionPlugin);

    // StealthPlugin injects utils.js
    protected override string[] RequiredPlugins => [nameof(StealthPlugin)];

    public async Task OnPageCreated(IPage page) {
        await page.EvaluateExpressionOnNewDocumentAsync(Scripts.Plugin);
    }
}