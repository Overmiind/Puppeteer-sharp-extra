using PuppeteerSharp;

namespace PuppeteerExtraSharpLite.Plugins.Stealth;

public class CodecPlugin : PuppeteerExtraPlugin, IOnPageCreatedPlugin {
    public override string Name => nameof(CodecPlugin);

    // StealthPlugin injects utils.js
    protected override string[] RequiredPlugins => [nameof(StealthPlugin)];

    public Task OnPageCreated(IPage page) {
        return page.EvaluateExpressionOnNewDocumentAsync(Scripts.Codec);
    }
}