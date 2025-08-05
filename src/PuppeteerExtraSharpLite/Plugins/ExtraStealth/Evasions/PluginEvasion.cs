using PuppeteerSharp;

namespace PuppeteerExtraSharpLite.Plugins.ExtraStealth.Evasions;

public class PluginEvasion : PuppeteerExtraPlugin, IOnPageCreatedPlugin {
    public override string Name => nameof(PluginEvasion);

    // StealthPlugin injects utils.js
    protected override string[] RequiredPlugins => [nameof(StealthPlugin)];

    public async Task OnPageCreated(IPage page) {
        await page.EvaluateExpressionOnNewDocumentAsync(Scripts.Plugin);
    }
}