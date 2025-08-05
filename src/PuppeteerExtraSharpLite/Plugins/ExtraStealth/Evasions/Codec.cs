using PuppeteerSharp;

namespace PuppeteerExtraSharpLite.Plugins.ExtraStealth.Evasions;

public class Codec : PuppeteerExtraPlugin, IOnPageCreatedPlugin {
    public override string Name => nameof(Codec);

    // StealthPlugin injects utils.js
    protected override string[] RequiredPlugins => [nameof(StealthPlugin)];

    public Task OnPageCreated(IPage page) {
        return page.EvaluateExpressionOnNewDocumentAsync(Scripts.Codec);
    }
}