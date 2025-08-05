using PuppeteerSharp;

namespace PuppeteerExtraSharpLite.Plugins.ExtraStealth.Evasions;

public class PluginEvasion : PuppeteerExtraPlugin, IOnPageCreatedPlugin {
    public override string Name => nameof(PluginEvasion);

    public PluginEvasion() : base() {
    }

    public async Task OnPageCreated(IPage page) {
        await page.EvaluateExpressionOnNewDocumentAsync(Scripts.Plugin);
    }
}